using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.NodeData;
using UnityEngine;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public class StateNode : GraphNode
    {
        public override string Guid => NodeData.Guid;

        internal StateNodeData NodeData { get; }

        internal List<StateTransitionEdge> OutputTransitions { get; } = new List<StateTransitionEdge>();

        private readonly List<StateTransitionEdge> _inputTransitions = new List<StateTransitionEdge>();

        private const float _CONNECTION_HANDLER_WIDTH = 8;


        public StateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData) : base(graphAsset)
        {
            NodeData = nodeData;

            // Styles
            mainContainer.style.backgroundColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1.0f);
            mainContainer.style.justifyContent = Justify.Center;
            mainContainer.style.borderTopWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderBottomWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderLeftWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderRightWidth = _CONNECTION_HANDLER_WIDTH;
            var titleLabel = titleContainer.Q<Label>(name: "title-label");
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            SetPosition(new Rect(NodeData.EditorPosition, Vector2.zero));
            RefreshPorts();
            RefreshExpandedState();

            // Callbacks
            mainContainer.AddManipulator(new StateTransitionEdgeConnector());
        }


        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (evt is GeometryChangedEvent)
            {
                foreach (var edge in OutputTransitions)
                {
                    edge.UpdateEdgeControl();
                }

                foreach (var edge in _inputTransitions)
                {
                    edge.UpdateEdgeControl();
                }
            }
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }

        public StateTransitionEdge AddTransition(StateNode destNode, StateTransitionEdge newEdge = null)
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
                OutputTransitions.Add(edge);
                destNode._inputTransitions.Add(edge);
                return edge;
            }

            // New transition
            if (newEdge != null)
            {
                newEdge.SetConnection(0, this);
                newEdge.SetConnection(1, destNode);
                edge = newEdge;
            }
            else
            {
                edge = new StateTransitionEdge(this, destNode);
            }

            OutputTransitions.Add(edge);
            destNode._inputTransitions.Add(edge);

            return edge;
        }

        public StateTransitionEdge RemoveTransition(StateNode destNode)
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
            }

            return removedEdge;
        }
    }
}
