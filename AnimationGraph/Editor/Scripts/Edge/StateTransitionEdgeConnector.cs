using GBG.AnimationGraph.Editor.GraphView;
using GBG.AnimationGraph.Editor.Node;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    public class StateTransitionEdgeConnector : MouseManipulator
    {
        private readonly AnimationGraphAsset _graphAsset;

        private bool _active;

        private StateNode _fromNode;

        private StateTransitionEdge _dragEdge;

        private StateMachineGraphView _graphView;


        public StateTransitionEdgeConnector(AnimationGraphAsset graphAsset)
        {
            _graphAsset = graphAsset;
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

            _fromNode = target as StateNode;
            if (_fromNode == null)
            {
                _fromNode = target.GetFirstAncestorOfType<StateNode>();
                if (_fromNode == null)
                {
                    return;
                }
            }

            _dragEdge = new StateTransitionEdge(_graphAsset, _fromNode, null);
            _dragEdge.SetEnabled(false);
            _graphView.AddElement(_dragEdge);

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

            _dragEdge.Drag(e.mousePosition);
            e.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!_active || !CanStopManipulation(e))
            {
                return;
            }

            var compatibleNodes = _graphView.GetCompatibleStateNodes(_dragEdge.ConnectedNode0);
            StateNode destNode = null;
            foreach (var node in compatibleNodes)
            {
                if (node.worldBound.Contains(e.mousePosition))
                {
                    destNode = node;
                    break;
                }
            }

            if (destNode != null)
            {
                var edge = _fromNode.AddTransition(destNode, out var dataDirty);
                _graphView.AddStateTransitionEdge(edge, dataDirty);
            }

            Abort();
            e.StopPropagation();
        }

        private void OnCaptureOut(MouseCaptureOutEvent e)
        {
            _active = false;

            if (_dragEdge != null)
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

            e.StopPropagation();
        }

        private void Abort()
        {
            _graphView.RemoveElement(_dragEdge);
            _dragEdge.ConnectedNode0 = null;
            _dragEdge.ConnectedNode1 = null;
            _dragEdge = null;
            _fromNode = null;

            _active = false;
            target.ReleaseMouse();
        }
    }
}
