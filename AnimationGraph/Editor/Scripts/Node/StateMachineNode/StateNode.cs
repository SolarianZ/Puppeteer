using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public class StateNode : GraphNode
    {
        public override string Guid => NodeData.Guid;

        public string MixerGraphGuid => NodeData.MixerGraphGuid;

        public virtual string StateName
        {
            get => GraphData.Name;
            internal set
            {
                GraphData.Name = value;
                title = GraphData.Name;
            }
        }

        internal StateNodeData NodeData { get; }

        internal GraphData.GraphData GraphData { get; }


        private const float _CONNECTION_HANDLER_WIDTH = 12;


        public StateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData,
            GraphData.GraphData graphData) : base(graphAsset)
        {
            Assert.AreEqual(nodeData.MixerGraphGuid, graphData.Guid);

            NodeData = nodeData;
            GraphData = graphData;

            // Styles
            mainContainer.style.backgroundColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1.0f);
            mainContainer.style.justifyContent = Justify.Center;
            mainContainer.style.borderTopWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderBottomWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderLeftWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderRightWidth = _CONNECTION_HANDLER_WIDTH;
            var titleLabel = titleContainer.Q<Label>(name: "title-label");
            titleLabel.style.maxWidth = 150;
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            titleLabel.style.whiteSpace = WhiteSpace.Normal;

            SetPosition(new Rect(NodeData.EditorPosition, Vector2.zero));
            RefreshPorts();
            RefreshExpandedState();

            // Callbacks
            mainContainer.AddManipulator(new StateTransitionEdgeConnector(GraphAsset));
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new StateNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
        }


        #region Transitions // TODO OPTIMIZABLE

        internal List<StateTransitionEdge> OutputTransitions { get; } = new List<StateTransitionEdge>();


        private readonly List<StateTransitionEdge> _inputTransitions = new List<StateTransitionEdge>();


        public virtual StateTransitionEdge AddTransition(StateNode destNode, out bool dataDirty)
        {
            var edge = ViewOnlyConnect(destNode);

            // Add transition data
            var transitionData = NodeData.Transitions.FirstOrDefault(t => t.DestStateGuid.Equals(destNode.Guid));
            if (transitionData == null)
            {
                transitionData = new Transition(destNode.Guid);
                NodeData.Transitions.Add(transitionData);
                dataDirty = true;
            }
            else
            {
                dataDirty = false;
            }

            return edge;
        }

        public StateTransitionEdge RemoveTransition(StateNode destNode)
        {
            var removedEdge = ViewOnlyDisconnect(destNode);
            if (removedEdge != null)
            {
                // Remove transition data
                var index = NodeData.Transitions.FindIndex(t => t.DestStateGuid.Equals(destNode.Guid));
                NodeData.Transitions.RemoveAt(index);
            }

            return removedEdge;
        }

        public bool RemoveTransition(StateTransitionEdge transitionEdge)
        {
            if (OutputTransitions.Remove(transitionEdge))
            {
                if (transitionEdge.TryGetConnectedNode(this, out var destNode))
                {
                    destNode._inputTransitions.Remove(transitionEdge);
                }

                // Remove transition data
                var index = NodeData.Transitions.FindIndex(t => t.DestStateGuid.Equals(destNode.Guid));
                NodeData.Transitions.RemoveAt(index);

                return true;
            }

            return false;
        }

        public StateTransitionEdge ViewOnlyConnect(StateNode destNode)
        {
            var edge = OutputTransitions.FirstOrDefault(e => e.IsConnection(this, destNode));

            // Transition already exists
            if (edge != null)
            {
                return edge;
            }

            // Reversed transition already exists
            edge = destNode.OutputTransitions.FirstOrDefault(e => e.IsConnection(this, destNode));
            if (edge != null)
            {
                edge.AddDirection(StateTransitionEdgeDirections.Bidirectional);
                OutputTransitions.Add(edge);
                destNode._inputTransitions.Add(edge);
                return edge;
            }

            // New transition
            edge = new StateTransitionEdge(GraphAsset, this, destNode);
            edge.AddDirection(StateTransitionEdgeDirections.Dir_0_1);
            edge.IsEntryEdge = false;

            OutputTransitions.Add(edge);
            destNode._inputTransitions.Add(edge);

            return edge;
        }

        public StateTransitionEdge ViewOnlyDisconnect(StateNode destNode)
        {
            StateTransitionEdge removedEdge = null;
            for (int i = 0; i < OutputTransitions.Count; i++)
            {
                var edge = OutputTransitions[i];
                if (edge.IsConnection(this, destNode))
                {
                    OutputTransitions.RemoveAt(i);
                    removedEdge = edge;
                    break;
                }
            }

            if (removedEdge != null)
            {
                destNode._inputTransitions.Remove(removedEdge);

                if (destNode.OutputTransitions.Contains(removedEdge))
                {
                    if (removedEdge.ConnectedNode1 == destNode)
                    {
                        removedEdge.RemoveDirection(StateTransitionEdgeDirections.Dir_0_1);
                    }
                    else
                    {
                        removedEdge.RemoveDirection(StateTransitionEdgeDirections.Dir_1_0);
                    }
                }
                else
                {
                    removedEdge.SetConnection(0, null);
                    removedEdge.SetConnection(1, null);
                    removedEdge.RemoveDirection(StateTransitionEdgeDirections.Bidirectional);
                }
            }

            return removedEdge;
        }

        public List<StateTransitionEdge> ViewOnlyDisconnectAll()
        {
            var edgesToRemove = new List<StateTransitionEdge>(OutputTransitions.Count + _inputTransitions.Count);
            for (int i = 0; i < OutputTransitions.Count; i++)
            {
                var edge = OutputTransitions[i];
                edge.RemoveDirection(StateTransitionEdgeDirections.Bidirectional);
                edgesToRemove.Add(edge);
                if (edge.TryGetConnectedNode(this, out var destNode))
                {
                    destNode._inputTransitions.Remove(edge);
                }
            }

            OutputTransitions.Clear();

            for (int i = 0; i < _inputTransitions.Count; i++)
            {
                var edge = _inputTransitions[i];
                edge.RemoveDirection(StateTransitionEdgeDirections.Bidirectional);
                if (!edgesToRemove.Contains(edge))
                {
                    edgesToRemove.Add(edge);
                }

                if (edge.TryGetConnectedNode(this, out var fromNode))
                {
                    fromNode.OutputTransitions.Remove(edge);
                }
            }

            _inputTransitions.Clear();

            return edgesToRemove;
        }

        #endregion
    }
}
