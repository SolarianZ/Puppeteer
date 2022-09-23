using System;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    public sealed class StateTransitionEdge : GraphElement
    {
        public int ConnectionCount
        {
            get
            {
                var count = 0;
                if (ConnectedNode0 != null) count++;
                if (ConnectedNode1 != null) count++;
                return count;
            }
        }

        public StateNode ConnectedNode0 { get; internal set; }

        public StateNode ConnectedNode1 { get; internal set; }

        // TODO: Transition data

        public StateTransitionEdgeControl EdgeControl { get; }


        private Vector2 _dragPoint;

        private static readonly Color _edgeNormalColor = Color.white;

        private static readonly Color _edgeHoverColor = new Color(68 / 255f, 192 / 255f, 255 / 255f);

        private static readonly Color _edgeSelectedColor = new Color(68 / 255f, 192 / 255f, 255 / 255f);


        public StateTransitionEdge(StateNode node0, StateNode node1)
        {
            ConnectedNode0 = node0;
            ConnectedNode1 = node1;
            _dragPoint = ConnectedNode0.worldBound.center;

            pickingMode = PickingMode.Position;
            style.flexGrow = 0;
            style.flexShrink = 0;
            style.position = Position.Absolute;
            style.width = 2; // For debugging
            style.height = 2; // For debugging
            AddToClassList("edge");

            var edgeStyleSheet = Resources.Load<StyleSheet>("AnimationGraph/StateTransitionEdge");
            styleSheets.Add(edgeStyleSheet);

            EdgeControl = new StateTransitionEdgeControl(GetPoint0(), GetPoint1());
            Add(EdgeControl);

            if (ConnectionCount == 2)
            {
                ResetLayer();
            }

            capabilities |= Capabilities.Selectable | Capabilities.Deletable;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            // TODO FIXME: Fix edge position
            RegisterCallback<CustomStyleResolvedEvent>(_ => schedule.Execute(UpdateEdgeControl));
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            return EdgeControl.ContainsPoint(this.ChangeCoordinatesTo(EdgeControl, localPoint));
        }

        public override bool Overlaps(Rect rectangle)
        {
            return EdgeControl.Overlaps(this.ChangeCoordinatesTo(EdgeControl, rectangle));
        }

        // TODO: Start inspecting transition
        public override void OnSelected()
        {
            EdgeControl.EdgeColor = _edgeSelectedColor;
            UpdateEdgeControl();

            Debug.LogError("TODO: Edge OnSelected");
        }

        // TODO: Stop inspecting transition
        public override void OnUnselected()
        {
            EdgeControl.EdgeColor = _edgeNormalColor;
            UpdateEdgeControl();

            Debug.LogError("TODO: Edge OnUnselected");
        }

        public void Drag(Vector2 mousePosition)
        {
            _dragPoint = mousePosition;
            UpdateEdgeControl();
        }

        public void SetConnection(int index, StateNode node)
        {
            switch (index)
            {
                case 0:
                    ConnectedNode0 = node;
                    break;

                case 1:
                    ConnectedNode1 = node;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (ConnectionCount == 2)
            {
                ResetLayer();
            }

            UpdateEdgeControl();
        }

        public bool IsConnection(StateNode a, StateNode b)
        {
            return (ConnectedNode0 == a && ConnectedNode1 == b) || (ConnectedNode0 == b && ConnectedNode1 == a);
        }

        public void UpdateEdgeControl()
        {
            EdgeControl.EdgePoint0 = EdgeControl.WorldToLocal(GetPoint0());
            EdgeControl.EdgePoint1 = EdgeControl.WorldToLocal(GetPoint1());
            EdgeControl.UpdateView();
        }


        private Vector2 GetPoint0()
        {
            return ConnectedNode0?.worldBound.center ?? default;
        }

        private Vector2 GetPoint1()
        {
            return ConnectedNode1?.worldBound.center ?? _dragPoint;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (!enabledInHierarchy || selected) return;

            EdgeControl.EdgeColor = _edgeHoverColor;
            UpdateEdgeControl();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (!enabledInHierarchy || selected) return;

            EdgeControl.EdgeColor = _edgeNormalColor;
            UpdateEdgeControl();
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Debug.LogError("TODO: Edge BuildContextualMenu");
        }
    }
}
