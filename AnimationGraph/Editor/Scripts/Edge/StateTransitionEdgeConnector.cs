using System.Linq;
using GBG.AnimationGraph.Editor.GraphView;
using GBG.AnimationGraph.Editor.Node;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    public class StateTransitionEdgeConnector : MouseManipulator
    {
        private bool _active;

        private StateTransitionEdge _edgeCandidate;

        private StateMachineGraphView _graphView;


        public StateTransitionEdgeConnector()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            target.UnregisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
        }


        private void OnMouseDown(MouseDownEvent e)
        {
            if (_active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (!CanStartManipulation(e))
            {
                return;
            }

            if (_graphView == null)
            {
                _graphView = target.GetFirstAncestorOfType<StateMachineGraphView>();
                if (_graphView == null)
                {
                    return;
                }
            }

            var node0 = target as StateNode;
            if (node0 == null)
            {
                node0 = target.GetFirstAncestorOfType<StateNode>();
                if (node0 == null)
                {
                    return;
                }
            }

            _edgeCandidate = new StateTransitionEdge(node0, null);
            _edgeCandidate.SetEnabled(false);
            _graphView.AddElement(_edgeCandidate);

            _active = true;
            target.CaptureMouse();

            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!_active)
            {
                return;
            }

            _edgeCandidate.Drag(e.mousePosition);
            e.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!_active || !CanStopManipulation(e))
            {
                return;
            }

            var compatibleNodes = _graphView.GetCompatibleStateNodes(_edgeCandidate.ConnectedNode0);
            StateNode node1 = null;
            foreach (var node in compatibleNodes)
            {
                if (node.worldBound.Contains(e.mousePosition))
                {
                    node1 = node;
                    break;
                }
            }

            if (node1 != null)
            {
                ConnectNode1(node1);
            }
            else
            {
                Abort();
            }

            _active = false;
            _edgeCandidate = null;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        private void OnCaptureOut(MouseCaptureOutEvent e)
        {
            _active = false;

            if (_edgeCandidate != null)
            {
                Abort();
            }
        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode != KeyCode.Escape || !_active)
            {
                return;
            }

            Abort();

            _active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        private void Abort()
        {
            _graphView.RemoveElement(_edgeCandidate);
            _edgeCandidate.ConnectedNode0 = null;
            _edgeCandidate.ConnectedNode1 = null;
            _edgeCandidate = null;
        }

        private void ConnectNode1(StateNode node1)
        {
            var edge = _edgeCandidate.ConnectedNode0.OutputTransitions.FirstOrDefault(e =>
                e.IsConnection(_edgeCandidate.ConnectedNode0, node1));

            // Transition already exists
            if (edge != null)
            {
                Abort();
                return;
            }

            // Reversed transition already exists
            edge = node1.OutputTransitions.FirstOrDefault(e =>
                e.IsConnection(_edgeCandidate.ConnectedNode0, node1));
            if (edge != null)
            {
                _edgeCandidate.ConnectedNode0.OutputTransitions.Add(edge);
                Abort();
                return;
            }

            // New transition
            _edgeCandidate.SetConnection(1, node1);
            _edgeCandidate.ConnectedNode0.AddTransition(node1, _edgeCandidate);
            _edgeCandidate.SetEnabled(true);
        }
    }
}
