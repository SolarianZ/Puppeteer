using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class MixerGraphView : GraphViewBase
    {
        public override GraphNode RootNode => PoseOutputNode;

        public PoseOutputNode PoseOutputNode { get; }


        public MixerGraphView(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, graphData)
        {
            // Root node
            PoseOutputNode = new PoseOutputNode(GraphAsset, graphData);
            AddElement(PoseOutputNode);

            // Nodes
            var nodeTable = new Dictionary<string, PlayableNode>(GraphData.Nodes.Count + 1);
            foreach (var nodeData in GraphData.Nodes)
            {
                var node = PlayableNodeFactory.CreateNode(GraphAsset, (PlayableNodeData)nodeData);
                AddElement(node);
                nodeTable.Add(node.Guid, node);
            }

            // Edges
            if (!string.IsNullOrEmpty(PoseOutputNode.RootPlayableNodeGuid))
            {
                var rootPlayableNode = nodeTable[PoseOutputNode.RootPlayableNodeGuid];
                var edge = PoseOutputNode.PoseInputPort.ConnectTo<FlowingGraphEdge>(rootPlayableNode.OutputPort);
                AddElement(edge);
            }

            foreach (var playableNode in nodeTable.Values)
            {
                ConnectNodeChildren(playableNode, nodeTable);
            }

            // Callbacks
            graphViewChanged += OnGraphViewChanged;
        }


        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (selection.Count == 0)
            {
                // Build menu items
                var localMousePos = contentViewContainer.WorldToLocal(evt.mousePosition);
                foreach (var nodeType in PlayableNodeFactory.GetPlayableNodeTypes())
                {
                    evt.menu.AppendAction($"Create {nodeType.Name}", _ =>
                    {
                        var node = PlayableNodeFactory.CreateNode(GraphAsset, nodeType, localMousePos);
                        if (node != null)
                        {
                            GraphData.Nodes.Add(node.NodeData);
                            AddElement(node);
                            RaiseGraphViewChangedEvent();
                        }
                    });
                }

                evt.menu.AppendSeparator();
            }
        }

        public override List<UPort> GetCompatiblePorts(UPort startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<UPort>();
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


        private void ConnectNodeChildren(PlayableNode parentNode, Dictionary<string, PlayableNode> nodeTable)
        {
            // TODO
            return;
            Assert.AreEqual(parentNode.NodeData.MixerInputs.Count, parentNode.InputPorts.Count);

            for (var i = 0; i < parentNode.InputPorts.Count; i++)
            {
                var inputPort = parentNode.InputPorts[i];
                var childNode = nodeTable[parentNode.NodeData.MixerInputs[i].InputNodeGuid];
                var edge = inputPort.ConnectTo<FlowingGraphEdge>(childNode.OutputPort);
                AddElement(edge);
            }
        }

        private new GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                if (element is PlayableNode playableNode)
                {
                    for (int i = 0; i < GraphData.Nodes.Count; i++)
                    {
                        if (GraphData.Nodes[i].Guid == playableNode.Guid)
                        {
                            GraphData.Nodes.RemoveAt(i);
                            break;
                        }
                    }
                }
            });

            RaiseGraphViewChangedEvent();

            return graphViewChange;
        }
    }
}
