using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
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
                var node = StateNodeFactory.CreateNode(GraphAsset, (StateNodeData)nodeData);
                AddElement(node);
                nodeTable.Add(node.Guid, node);
            }

            // Edges
            if (!string.IsNullOrEmpty(StateMachineEntryNode.DestStateNodeGuid))
            {
                var rootStateNode = nodeTable[StateMachineEntryNode.DestStateNodeGuid];
                var edge = StateMachineEntryNode.ViewOnlyConnect(rootStateNode);
                edge.IsEntryEdge = true;
                AddElement(edge);
            }

            foreach (var stateNode in nodeTable.Values)
            {
                foreach (var transition in stateNode.NodeData.Transitions)
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

        public List<StateNode> GetCompatibleStateNodes(StateNode fromNode)
        {
            var nodeList = new List<StateNode>();
            foreach (var node in nodes)
            {
                if (node == fromNode || node is StateMachineEntryNode)
                {
                    continue;
                }

                nodeList.Add((StateNode)node);
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
                    evt.menu.AppendAction($"Create {nodeType.Name}", _ =>
                    {
                        var node = StateNodeFactory.CreateNode(GraphAsset, nodeType, localMousePos);
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

        public void AddStateTransitionEdge(StateTransitionEdge edge, bool forceRaiseGraphViewChangedEvent)
        {
            if (!Contains(edge))
            {
                AddElement(edge);
                RaiseGraphViewChangedEvent();
            }
            else if (forceRaiseGraphViewChangedEvent)
            {
                RaiseGraphViewChangedEvent();
            }
        }


        private new GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
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
                    }
                }
            }

            RaiseGraphViewChangedEvent();

            return graphViewChange;
        }
    }
}
