using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public class PaneDraggerManipulator : MouseManipulator
    {
        private readonly VisualElement _targetPane;

        private readonly FlexDirection _flexDir;

        private readonly int _sign;

        private bool _isActive;

        private Vector2 _start;

        private float _oldPaneSize;


        public PaneDraggerManipulator(VisualElement targetPane, FlexDirection flexDir, int sign)
        {
            _targetPane = targetPane;
            _flexDir = flexDir;
            _sign = sign;

            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse,
            });
        }

        public void ApplyDelta(float delta)
        {
            var newPaneSize = _oldPaneSize + delta;
            if (_flexDir == FlexDirection.Row)
            {
                _targetPane.style.width = newPaneSize;
            }
            else
            {
                _targetPane.style.height = newPaneSize;
            }
        }


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }


        private void OnMouseDown(MouseDownEvent e)
        {
            if (_isActive)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                _isActive = true;
                _start = e.mousePosition;
                _oldPaneSize = _flexDir == FlexDirection.Row
                    ? _targetPane.resolvedStyle.width
                    : _targetPane.resolvedStyle.height;

                target.CaptureMouse();

                e.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!_isActive || !target.HasMouseCapture())
            {
                return;
            }

            Vector2 mouseDelta = e.mousePosition - _start;
            float sizeDelta = (_flexDir == FlexDirection.Row ? mouseDelta.x : mouseDelta.y) * _sign;
            ApplyDelta(sizeDelta);

            e.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (!_isActive || !target.HasMouseCapture() || !CanStopManipulation(e))
            {
                return;
            }

            _isActive = false;

            target.ReleaseMouse();

            e.StopPropagation();
        }
    }
}
