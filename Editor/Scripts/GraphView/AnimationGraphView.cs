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
using UnityGraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GBG.Puppeteer.Editor.GraphView
{
    public class AnimationGraphView : UnityGraphView
    {
        private readonly RootNode _rootNode;

        private readonly List<ParamInfo> _paramInfos = new List<ParamInfo>();


        public AnimationGraphView(RuntimeAnimationGraph graphAsset)
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Root node
            _rootNode = new RootNode();
            AddElement(_rootNode);

            // Parameters
            foreach (var param in graphAsset.Parameters)
            {
                _paramInfos.Add((ParamInfo)param.Clone());
                // TODO: Add parameter to blackboard
            }

            // Active nodes
            foreach (var nodeData in graphAsset.EditorNodes)
            {
                var node = PlayableNodeFactory.InstantiateNode(nodeData);
                if (node != null)
                {
                    AddElement(node);
                }
            }

            // TODO: Link active nodes

            // Isolated nodes
            foreach (var nodeData in graphAsset.EditorIsolatedNodes)
            {
                var node = PlayableNodeFactory.InstantiateNode(nodeData);
                if (node != null)
                {
                    AddElement(node);
                }
            }

            // Graph view change callback
            graphViewChanged += OnGraphViewChanged;
        }


        public void SaveToGraphAsset(RuntimeAnimationGraph graphAsset)
        {
            // Parameters
            graphAsset.EditorParameters = new ParamInfo[_paramInfos.Count()];
            for (int i = 0; i < _paramInfos.Count(); i++)
            {
                graphAsset.EditorParameters[i] = (ParamInfo)_paramInfos[i].Clone();
            }

            // Nodes
            var nodeDict = new Dictionary<string, PlayableNode>(nodes.Count());
            foreach (var node in nodes)
            {
                if (node is PlayableNode animGraphNode)
                {
                    nodeDict.Add(animGraphNode.Guid, animGraphNode);
                }
            }

            // Active nodes
            var activeNodeDataList = new List<AnimationNodeData>(nodes.Count());
            CollectNodeDataRecursively(_rootNode.Input, nodeDict, activeNodeDataList);
            graphAsset.EditorNodes = activeNodeDataList.ToArray();

            // Isolated nodes
            graphAsset.EditorIsolatedNodes = new AnimationNodeData[nodeDict.Count];
            var index = 0;
            foreach (var isolatedNode in nodeDict.Values)
            {
                graphAsset.EditorIsolatedNodes[index] = isolatedNode.CloneNodeData();
                index++;
            }

            static void CollectNodeDataRecursively(PlayableNode rootNode,
                Dictionary<string, PlayableNode> nodeDict,
                List<AnimationNodeData> nodeDataList)
            {
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
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    if (edge is AnimationGraphEdge animEdge)
                    {
                        animEdge.Input.Node.OnInputConnected(animEdge);
                    }
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var elemToRemove in graphViewChange.elementsToRemove)
                {
                    if (elemToRemove is AnimationGraphEdge animEdge)
                    {
                        animEdge.Input.Node.OnInputDisconnected(animEdge);
                    }
                }
            }


            OnGraphChanged?.Invoke();

            return graphViewChange;
        }
    }
}
