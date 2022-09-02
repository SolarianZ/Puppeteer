using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public class TripleSplitterRowView : VisualElement
    {
        public VisualElement LeftPane { get; }

        public VisualElement MiddlePane { get; }

        public VisualElement RightPane { get; }

        public VisualElement LeftSplitter { get; }

        public VisualElement RightSplitter { get; }


        //  RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

        public TripleSplitterRowView(Vector2 leftPaneMinMaxWidth, Vector2 rightPaneMinMaxWidth
            , float splitterWidth = 2f)
        {
            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Row;

            // Left pane
            LeftPane = new VisualElement
            {
                name = "left-pane",
                style =
                {
                    minWidth = leftPaneMinMaxWidth.x,
                    maxWidth = leftPaneMinMaxWidth.y,
                    width = (leftPaneMinMaxWidth.x + leftPaneMinMaxWidth.y) / 2,
                    height = Length.Percent(100),
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                    // backgroundColor = new Color(1, 0, 0, 0.5f), // TEST
                }
            };
            Add(LeftPane);

            // Left splitter
            LeftSplitter = new VisualElement
            {
                name = "left-splitter",
                style =
                {
                    width = splitterWidth,
                    height = Length.Percent(100),
                    backgroundColor = Color.black,
                    cursor = LoadCursor(MouseCursor.SplitResizeLeftRight),
                }
            };
            Add(LeftSplitter);

            // Middle pane
            MiddlePane = new VisualElement()
            {
                name = "middle-pane",
                style =
                {
                    flexGrow = 1,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                    // backgroundColor = new Color(0, 1, 0, 0.5f), // TEST
                }
            };
            Add(MiddlePane);

            // Right splitter
            RightSplitter = new VisualElement
            {
                name = "right-splitter",
                style =
                {
                    width = splitterWidth,
                    height = Length.Percent(100),
                    backgroundColor = Color.black,
                    cursor = LoadCursor(MouseCursor.SplitResizeLeftRight),
                }
            };
            Add(RightSplitter);

            // Right pane
            RightPane = new VisualElement()
            {
                name = "right-pane",
                style =
                {
                    minWidth = rightPaneMinMaxWidth.x,
                    maxWidth = rightPaneMinMaxWidth.y,
                    width = (rightPaneMinMaxWidth.x + rightPaneMinMaxWidth.y) / 2,
                    height = Length.Percent(100),
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                    // backgroundColor = new Color(0, 0, 1, 0.5f), // TEST
                }
            };
            Add(RightPane);

            // Left dragger
            var leftDragger = new PaneDraggerManipulator(LeftPane, FlexDirection.Row, 1);
            LeftSplitter.AddManipulator(leftDragger);

            // Right dragger
            var rightDragger = new PaneDraggerManipulator(RightPane, FlexDirection.Row, -1);
            RightSplitter.AddManipulator(rightDragger);
        }


        #region Toggle pane

        private Length _leftPaneMinWidth;

        private Length _leftPaneWidth;

        private Length _leftSplitterWidth;

        public void ToggleLeftPane(bool isVisible)
        {
            if (isVisible)
            {
                LeftPane.style.minWidth = _leftPaneMinWidth;
                LeftPane.style.width = _leftPaneWidth;
                LeftSplitter.style.width = _leftSplitterWidth;
            }
            else
            {
                _leftPaneMinWidth = LeftPane.style.minWidth.value;
                _leftPaneWidth = LeftPane.style.width.value;
                _leftSplitterWidth = LeftSplitter.style.width.value;

                LeftPane.style.minWidth = 0;
                LeftPane.style.width = 0;
                LeftSplitter.style.width = 0;
            }

            LeftPane.visible = isVisible;
            LeftPane.SetEnabled(isVisible);
            LeftSplitter.SetEnabled(isVisible);
        }


        private Length _rightPaneMinWidth;

        private Length _rightPaneWidth;

        private Length _rightSplitterWidth;

        public void ToggleRightPane(bool isVisible)
        {
            if (isVisible)
            {
                RightPane.style.minWidth = _rightPaneMinWidth;
                RightPane.style.width = _rightPaneWidth;
                RightSplitter.style.width = _rightSplitterWidth;
            }
            else
            {
                _rightPaneMinWidth = RightPane.style.minWidth.value;
                _rightPaneWidth = RightPane.style.width.value;
                _rightSplitterWidth = RightSplitter.style.width.value;

                RightPane.style.minWidth = 0;
                RightPane.style.width = 0;
                RightSplitter.style.width = 0;
            }

            RightPane.visible = isVisible;
            RightPane.SetEnabled(isVisible);
            RightSplitter.SetEnabled(isVisible);
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
