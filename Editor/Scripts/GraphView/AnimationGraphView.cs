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
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityGraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GBG.Puppeteer.Editor.GraphView
{
    public class AnimationGraphView : UnityGraphView
    {
        private readonly RootNode _rootNode;


        public bool SuppressGraphViewChangedEvent { get; set; }


        public AnimationGraphView(RuntimeAnimationGraph graphAsset)
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


        public event Action OnGraphChanged;

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (!SuppressGraphViewChangedEvent)
            {
                OnGraphChanged?.Invoke();
            }

            return graphViewChange;
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
    }
}
