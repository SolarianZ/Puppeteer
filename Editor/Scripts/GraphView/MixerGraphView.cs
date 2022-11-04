using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class MixerGraphView : GraphViewBase
    {
        public override GraphEditorNode RootNode => PoseOutputNode;

        public PoseOutputEditorNode PoseOutputNode { get; }


        public MixerGraphView(AnimationGraphAsset graphAsset, GraphLayer graphLayer)
            : base(graphAsset, graphLayer)
        {
            // Root node
            PoseOutputNode = new PoseOutputEditorNode(GraphAsset, graphLayer);
            AddElement(PoseOutputNode);

            // Nodes
            var nodeTable = new Dictionary<string, MixerGraphEditorNode>(GraphLayer.Nodes.Count + 1);
            foreach (var nodeData in GraphLayer.Nodes)
            {
                var node = PlayableEditorNodeFactory.CreateNode(GraphAsset, nodeData, false);
                node.OnDoubleClicked += OnDoubleClickNode;
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
                foreach (var nodeType in PlayableEditorNodeFactory.GetPlayableNodeTypes())
                {
                    evt.menu.AppendAction($"Create {nodeType.Name}", _ =>
                    {
                        var node = PlayableEditorNodeFactory.CreateNode(GraphAsset, nodeType, localMousePos);
                        if (node != null)
                        {
                            node.OnDoubleClicked += OnDoubleClickNode;

                            GraphLayer.Nodes.Add(node.Node);
                            if (node is StateMachineEditorNode stateMachineNode)
                            {
                                GraphAsset.GraphLayers.Add(new GraphLayer(stateMachineNode.StateMachineGraphGuid,
                                    $"SubGraph_{GuidTool.NewUniqueSuffix()}", GraphType.StateMachine));
                            }

                            AddElement(node);
                            RaiseContentChangedEvent(DataCategories.GraphData);
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


        private void ConnectNodeChildren(MixerGraphEditorNode parentNode,
            Dictionary<string, MixerGraphEditorNode> nodeTable)
        {
            var parentNodeInputGuids = parentNode.Node.GetInputNodeGuids();
            Assert.AreEqual(parentNodeInputGuids.Count, parentNode.InputPorts.Count,
                $"Input guid count not match with input port count. Graph guid: {Guid}");

            for (var i = 0; i < parentNode.InputPorts.Count; i++)
            {
                var inputPort = parentNode.InputPorts[i];
                var childNodeGuid = parentNodeInputGuids[i];
                if (string.IsNullOrEmpty(childNodeGuid))
                {
                    continue;
                }

                var childNode = nodeTable[childNodeGuid];
                var edge = inputPort.ConnectTo<FlowingGraphEdge>(childNode.OutputPort);
                AddElement(edge);
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                if (element is MixerGraphEditorNode playableNode)
                {
                    for (int i = 0; i < GraphLayer.Nodes.Count; i++)
                    {
                        if (GraphLayer.Nodes[i].Guid == playableNode.Guid)
                        {
                            GraphLayer.Nodes.RemoveAt(i);
                            break;
                        }
                    }
                }
            });

            RaiseContentChangedEvent(DataCategories.GraphContent);

            return graphViewChange;
        }

        private void OnDoubleClickNode(GraphEditorNode graphNode)
        {
            if (graphNode is StateMachineEditorNode stateMachineNode)
            {
                RaiseWantsToOpenGraphEvent(stateMachineNode.StateMachineGraphGuid);
            }
        }
    }
}
