using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class StateMachineGraphView : GraphViewBase
    {
        public override GraphNode RootNode => StateMachineEntryNode;

        public StateMachineEntryNode StateMachineEntryNode { get; }


        public StateMachineGraphView(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, graphData)
        {
            // Root node
            StateMachineEntryNode = new StateMachineEntryNode(GraphAsset, GraphData);
            AddElement(StateMachineEntryNode);

            // Nodes
            var nodeTable = new Dictionary<string, StateNode>(GraphData.Nodes.Count + 1);
            foreach (var nodeData in GraphData.Nodes)
            {
                var stateNodeData = (StateNodeData)nodeData;
                var stateNodeGraphData = GraphAsset.Graphs.Find(data => data.Guid.Equals(stateNodeData.MixerGraphGuid));
                var node = StateNodeFactory.CreateNode(GraphAsset, stateNodeData, stateNodeGraphData);
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

        public List<StateGraphNode> GetCompatibleNodes(StateGraphNode fromNode)
        {
            var nodeList = new List<StateGraphNode>();
            foreach (var node in nodes)
            {
                if (node == fromNode || node is StateMachineEntryNode)
                {
                    continue;
                }

                nodeList.Add((StateGraphNode)node);
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
                foreach (var nodeType in StateNodeFactory.GetStateNodeTypes())
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
                var stateNode = StateNodeFactory.CreateNode(GraphAsset, nodeType, graphType,
                    localMousePosition, out var stateNodeGraphData);
                if (stateNode != null)
                {
                    stateNode.OnDoubleClicked += OnDoubleClickNode;

                    GraphAsset.Graphs.Add(stateNodeGraphData);
                    GraphData.Nodes.Add(stateNode.NodeData);
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
                    else if (element is StateNode stateNode)
                    {
                        // Node table
                        for (int j = 0; j < GraphData.Nodes.Count; j++)
                        {
                            if (GraphData.Nodes[j].Guid == stateNode.Guid)
                            {
                                GraphData.Nodes.RemoveAt(j);
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
                        for (int j = 0; j < GraphAsset.Graphs.Count; j++)
                        {
                            if (GraphAsset.Graphs[j].Guid.Equals(stateNode.Guid))
                            {
                                GraphAsset.Graphs.RemoveAt(j);
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

        private void OnDoubleClickNode(GraphNode graphNode)
        {
            if (graphNode is StateMachineEntryNode) return;

            var graphGuid = ((StateNode)graphNode).MixerGraphGuid;
            RaiseWantsToOpenGraphEvent(graphGuid);
        }
    }
}
