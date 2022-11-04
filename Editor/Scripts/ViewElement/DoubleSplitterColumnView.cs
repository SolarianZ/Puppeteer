using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace GBG.AnimationGraph.Editor.ViewElement
{
    public class DoubleSplitterColumnView : VisualElement
    {
        public VisualElement UpPane { get; }

        public VisualElement DownPane { get; }

        public VisualElement Splitter { get; }


        //  RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

        public DoubleSplitterColumnView(Vector2 upPaneMinMaxHeightPercent, float splitterHeight = 2f)
        {
            Assert.IsTrue(upPaneMinMaxHeightPercent.x <= upPaneMinMaxHeightPercent.y,
                $"Height range of up pane is negative({upPaneMinMaxHeightPercent.ToString()}).");

            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Column;

            var splitterColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1.0f);

            // Up pane
            UpPane = new VisualElement
            {
                name = "up-pane",
                style =
                {
                    width = Length.Percent(100),
                    height = Length.Percent(Mathf.Clamp(60, upPaneMinMaxHeightPercent.x, upPaneMinMaxHeightPercent.y)),
                    minHeight = Length.Percent(upPaneMinMaxHeightPercent.x),
                    maxHeight = Length.Percent(upPaneMinMaxHeightPercent.y),
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                }
            };
            Add(UpPane);

            // Splitter
            Splitter = new VisualElement
            {
                name = "splitter",
                style =
                {
                    width = Length.Percent(100),
                    height = splitterHeight,
                    backgroundColor = splitterColor,
                    cursor = LoadCursor(MouseCursor.SplitResizeUpDown),
                }
            };
            Add(Splitter);

            // Down pane
            DownPane = new VisualElement()
            {
                name = "down-pane",
                style =
                {
                    flexGrow = 1,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                }
            };
            Add(DownPane);

            // Dragger
            var dragger = new PaneDraggerManipulator(UpPane, FlexDirection.Column, 1);
            Splitter.AddManipulator(dragger);
        }


        #region Toggle pane

        private Length _upPaneMinHeight;

        private Length _upPaneHeight;

        private Length _splitterHeight;

        public void ToggleUpPane(bool isVisible)
        {
            if (isVisible)
            {
                UpPane.style.minHeight = _upPaneMinHeight;
                UpPane.style.height = _upPaneHeight;
                Splitter.style.height = _splitterHeight;
            }
            else
            {
                _upPaneMinHeight = UpPane.style.minHeight.value;
                _upPaneHeight = UpPane.style.height.value;
                _splitterHeight = Splitter.style.height.value;

                UpPane.style.minHeight = 0;
                UpPane.style.height = 0;
                Splitter.style.height = 0;
            }

            UpPane.visible = isVisible;
            UpPane.SetEnabled(isVisible);
            Splitter.SetEnabled(isVisible);
        }

        #endregion


        private static Cursor LoadCursor(MouseCursor cursorStyle)
        {
            object boxed = new Cursor();
            typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(boxed, (int)cursorStyle, null);

            return (Cursor)boxed;
        }
    }
}
