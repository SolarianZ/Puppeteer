using System;
using System.Collections.Generic;
using System.Linq;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphNode;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UDebug = UnityEngine.Debug;
using UnityGraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GBG.Puppeteer.Editor.GraphView
{
    public class AnimationGraphView : UnityGraphView
    {
        private readonly RootNode _rootNode;


        public AnimationGraphView()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Root node
            _rootNode = new RootNode();
            AddElement(_rootNode);

            // Graph view change callback
            graphViewChanged += OnGraphViewChanged;
        }


        #region Build Graph

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports)
            {
                if (port.node != startPort.node &&
                    port.direction != startPort.direction &&
                    port.portType == startPort.portType)
                {
                    compatiblePorts.Add(port);
                }
            }

            return compatiblePorts;
        }

        public void RebuildGraph(IList<AnimationNodeData> linkedNodes, IList<AnimationNodeData> isolatedNodes,
            List<ParamInfo> paramTable)
        {
            // Old elements
            foreach (var edge in edges)
            {
                RemoveElement(edge);
            }

            foreach (var node in nodes)
            {
                if (node == _rootNode)
                {
                    continue;
                }

                RemoveElement(node);
            }

            // Active nodes
            var nodeTable = new Dictionary<string, PlayableNode>(linkedNodes.Count);
            var nodeDataTable = new Dictionary<string, AnimationNodeData>(linkedNodes.Count);
            foreach (var nodeData in linkedNodes)
            {
                var node = PlayableNodeFactory.InstantiateNode(nodeData, paramTable);
                if (node != null)
                {
                    AddElement(node);
                    nodeTable.Add(node.Guid, node);
                    nodeDataTable.Add(nodeData.Guid, nodeData);
                }
            }

            // Isolated nodes
            foreach (var nodeData in isolatedNodes)
            {
                var node = PlayableNodeFactory.InstantiateNode(nodeData, paramTable);
                if (node != null)
                {
                    AddElement(node);
                }
            }

            // Link active nodes
            if (linkedNodes.Count > 0)
            {
                var rootPlayableNode = nodeTable[linkedNodes[0].Guid];
                var edge = _rootNode.InputPort.ConnectTo<AnimationGraphEdge>(rootPlayableNode.OutputPort);
                AddElement(edge);
                LinkNodes(rootPlayableNode, nodeTable, nodeDataTable);
            }
        }

        private void LinkNodes(PlayableNode rootNode, Dictionary<string, PlayableNode> nodeTable,
            Dictionary<string, AnimationNodeData> nodeDataTable)
        {
            if (rootNode == null)
            {
                return;
            }

            var rootNodeData = nodeDataTable[rootNode.Guid];
            if (rootNodeData.InputInfos == null)
            {
                return;
            }

            for (int i = 0; i < rootNodeData.InputInfos.Length; i++)
            {
                var inputNode = nodeTable[rootNodeData.InputInfos[i].InputNodeGuid];
                var edge = inputNode.OutputPort.ConnectTo<AnimationGraphEdge>(rootNode.InputPorts[i]);
                AddElement(edge);

                LinkNodes(inputNode, nodeTable, nodeDataTable);
            }
        }

        #endregion


        #region Context Menu

        private IEnumerable<Type> _nodeTypesCache;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var localMousePos = contentViewContainer.WorldToLocal(evt.mousePosition);
            if (selection.Count == 0)
            {
                // Collect available nodes
                _nodeTypesCache ??= from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where IsPlayableNodeType(type)
                    select type;

                // Build menu items
                var playableNodeCtorParamTypes = new Type[] { typeof(string) };
                foreach (var nodeType in _nodeTypesCache)
                {
                    evt.menu.AppendAction($"Create {nodeType.Name}", (action) =>
                    {
                        var ctor = nodeType.GetConstructor(playableNodeCtorParamTypes);
                        if (ctor == null)
                        {
                            UDebug.LogError($"[Puppeteer::GraphView] {nodeType.Name} does not have " +
                                            "a constructor with a single string parameter(guid).");
                            return;
                        }

                        var playableNodeCtorParams = new object[] { PlayableNode.NewGuid() };
                        var node = (PlayableNode)ctor.Invoke(playableNodeCtorParams);
                        node.SetPosition(new Rect(localMousePos, Vector2.zero));
                        AddElement(node);
                    });
                }
            }
            else if (!selection.Contains(_rootNode))
            {
                base.BuildContextualMenu(evt);
            }
        }

        static bool IsPlayableNodeType(Type type)
        {
            if (type.IsInterface || type.IsAbstract || (type.IsGenericType && !type.IsConstructedGenericType))
            {
                return false;
            }

            return typeof(PlayableNode).IsAssignableFrom(type);
        }

        #endregion


        #region Save & Load

        public void SaveNodesToGraphAsset(RuntimeAnimationGraph graphAsset)
        {
            // Node table
            var nodeTable = new Dictionary<string, PlayableNode>(nodes.Count());
            foreach (var node in nodes)
            {
                if (node is PlayableNode animGraphNode)
                {
                    nodeTable.Add(animGraphNode.Guid, animGraphNode);
                }
            }

            // Active nodes
            var activeNodeDataList = new List<AnimationNodeData>(nodes.Count());
            CollectNodeDataRecursively(_rootNode.InputNode, nodeTable, activeNodeDataList);
            graphAsset.EditorNodes = activeNodeDataList.ToArray();

            // Isolated nodes
            graphAsset.EditorIsolatedNodes = new AnimationNodeData[nodeTable.Count];
            var index = 0;
            foreach (var isolatedNode in nodeTable.Values)
            {
                graphAsset.EditorIsolatedNodes[index] = isolatedNode.CloneNodeData();
                index++;
            }

            static void CollectNodeDataRecursively(PlayableNode rootNode,
                Dictionary<string, PlayableNode> nodeDict,
                List<AnimationNodeData> nodeDataList)
            {
                if (rootNode == null)
                {
                    return;
                }

                nodeDict.Remove(rootNode.Guid);
                nodeDataList.Add(rootNode.CloneNodeData());

                foreach (var child in rootNode.InputNodes)
                {
                    CollectNodeDataRecursively(child, nodeDict, nodeDataList);
                }
            }
        }

        #endregion


        #region View Change Event

        public bool SuppressGraphViewChangedEvent { get; set; }

        public event Action OnGraphChanged;


        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // Disallow to delete root node
            graphViewChange.elementsToRemove?.Remove(_rootNode);

            // Disallow to move root node
            if (graphViewChange.movedElements?.Remove(_rootNode) ?? false)
            {
                var rootNodePos = _rootNode.GetPosition();
                rootNodePos.position -= graphViewChange.moveDelta;
                _rootNode.SetPosition(rootNodePos);
            }

            if (!SuppressGraphViewChangedEvent)
            {
                OnGraphChanged?.Invoke();
            }

            return graphViewChange;
        }

        #endregion
    }
}
