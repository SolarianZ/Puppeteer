using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private void CreateInspectorPanel()
        {
            // Title bar
            var titleBar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.black,
                }
            };
            _layoutContainer.RightPane.Add(titleBar);

            // Inspector label
            var inspectorLabel = new Label("Inspector")
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = 4,
                }
            };
            titleBar.Add(inspectorLabel);
        }
    }
}
