using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class StateMachineGraphView : GraphViewBase
    {
        public override GraphEditorNode RootNode => StateMachineEntryNode;

        public StateMachineEntryEditorNode StateMachineEntryNode { get; }


        public StateMachineGraphView(AnimationGraphAsset graphAsset, Graph.GraphLayer graphLayer)
            : base(graphAsset, graphLayer)
        {
            // Root node
            StateMachineEntryNode = new StateMachineEntryEditorNode(GraphAsset, GraphLayer);
            AddElement(StateMachineEntryNode);

            // Nodes
            var nodeTable = new Dictionary<string, StateEditorNode>(GraphLayer.Nodes.Count + 1);
            foreach (var nodeData in GraphLayer.Nodes)
            {
                var stateNodeData = (StateNode)nodeData;
                var stateNodeGraphData = GraphAsset.GraphLayers.Find(data => data.Guid.Equals(stateNodeData.MixerGraphGuid));
                var node = StateEditorNodeFactory.CreateNode(GraphAsset, stateNodeData, stateNodeGraphData);
                node.OnDoubleClicked += OnDoubleClickNode;
                nodeTable.Add(node.Guid, node);
                AddElement(node);
            }

            // Edges
            if (!string.IsNullOrEmpty(StateMachineEntryNode.DestStateNodeGuid))
            {
                var rootStateNode = nodeTable[StateMachineEntryNode.DestStateNodeGuid];
                var edge = StateMachineEntryNode.ViewOnlyConnect(rootStateNode);
                edge.IsEntryEdge = true;
                AddElement(edge);
            }

            // Transitions
            foreach (var stateNode in nodeTable.Values)
            {
                foreach (var transition in stateNode.Transitions)
                {
                    var destNode = nodeTable[transition.DestStateGuid];
                    var edge = stateNode.ViewOnlyConnect(destNode);
                    edge.IsEntryEdge = false;
                    AddElement(edge);
                }
            }

            // Callbacks
            graphViewChanged += OnGraphViewChanged;
        }

        public List<StateGraphEditorNode> GetCompatibleNodes(StateGraphEditorNode fromNode)
        {
            var nodeList = new List<StateGraphEditorNode>();
            foreach (var node in nodes)
            {
                if (node == fromNode || node is StateMachineEntryEditorNode)
                {
                    continue;
                }

                nodeList.Add((StateGraphEditorNode)node);
            }

            return nodeList;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (selection.Count == 0)
            {
                // Build menu items
                var localMousePos = contentViewContainer.WorldToLocal(evt.mousePosition);
                foreach (var nodeType in StateEditorNodeFactory.GetStateNodeTypes())
                {
                    // New mixer state
                    evt.menu.AppendAction($"Create Mixer {nodeType.Name}",
                        _ => CreateNode(nodeType, GraphType.Mixer, localMousePos));

                    // New sub state machine state
                    // TODO: Give sub state machine a different color
                    evt.menu.AppendAction($"Create Sub State Machine {nodeType.Name}",
                        _ => CreateNode(nodeType, GraphType.StateMachine, localMousePos));
                }

                evt.menu.AppendSeparator();
            }

            void CreateNode(Type nodeType, GraphType graphType, Vector2 localMousePosition)
            {
                var stateNode = StateEditorNodeFactory.CreateNode(GraphAsset, nodeType, graphType,
                    localMousePosition, out var stateNodeGraphData);
                if (stateNode != null)
                {
                    stateNode.OnDoubleClicked += OnDoubleClickNode;

                    GraphAsset.GraphLayers.Add(stateNodeGraphData);
                    GraphLayer.Nodes.Add(stateNode.Node);
                    AddElement(stateNode);
                    RaiseContentChangedEvent(DataCategories.GraphData);
                }
            }
        }

        public void AddStateTransitionEdge(StateTransitionEdge edge, bool forceRaiseGraphViewChangedEvent)
        {
            if (!Contains(edge))
            {
                AddElement(edge);
                RaiseContentChangedEvent(DataCategories.TransitionData);
            }
            else if (forceRaiseGraphViewChangedEvent)
            {
                RaiseContentChangedEvent(DataCategories.TransitionData);
            }
        }


        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            var changedDataCategories = DataCategories.None;
            if (graphViewChange.elementsToRemove != null)
            {
                for (int i = graphViewChange.elementsToRemove.Count - 1; i >= 0; i--)
                {
                    var element = graphViewChange.elementsToRemove[i];

                    // Delete edges
                    if (element is StateTransitionEdge edge)
                    {
                        edge.ConnectedNode0.RemoveTransition(edge);
                        edge.ConnectedNode1.RemoveTransition(edge);
                        changedDataCategories |= DataCategories.TransitionData;
                    }
                    // Delete nodes
                    else if (element is StateEditorNode stateNode)
                    {
                        // Node table
                        for (int j = 0; j < GraphLayer.Nodes.Count; j++)
                        {
                            if (GraphLayer.Nodes[j].Guid == stateNode.Guid)
                            {
                                GraphLayer.Nodes.RemoveAt(j);
                                break;
                            }
                        }

                        // Transitions
                        var edgesToRemove = stateNode.ViewOnlyDisconnectAll();
                        graphViewChange.elementsToRemove.AddRange(edgesToRemove);

                        // State machine entry
                        if (stateNode.Guid.Equals(StateMachineEntryNode.DestStateNodeGuid))
                        {
                            StateMachineEntryNode.DestStateNodeGuid = null;
                        }

                        // Graphs
                        for (int j = 0; j < GraphAsset.GraphLayers.Count; j++)
                        {
                            if (GraphAsset.GraphLayers[j].Guid.Equals(stateNode.Guid))
                            {
                                GraphAsset.GraphLayers.RemoveAt(j);
                                break;
                            }
                        }

                        changedDataCategories |= DataCategories.NodeData;
                    }
                }
            }

            if (graphViewChange.moveDelta.sqrMagnitude > Mathf.Epsilon)
            {
                changedDataCategories |= DataCategories.NodeData;
            }

            RaiseContentChangedEvent(changedDataCategories);

            return graphViewChange;
        }

        private void OnDoubleClickNode(GraphEditorNode graphNode)
        {
            if (graphNode is StateMachineEntryEditorNode) return;

            var graphGuid = ((StateEditorNode)graphNode).MixerGraphGuid;
            RaiseWantsToOpenGraphEvent(graphGuid);
        }
    }
}
