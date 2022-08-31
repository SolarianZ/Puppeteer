using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private VisualElement _inspectorPanel;


        private void CreateInspectorPanel()
        {
            _inspectorPanel = new VisualElement
            {
                name = "inspector-panel",
                style =
                {
                    width = 300,
                    height = Length.Percent(100),
                    borderLeftWidth = 1,
                    borderLeftColor = Color.black
                }
            };
            _layoutContainer.Add(_inspectorPanel);
        }
    }
}
