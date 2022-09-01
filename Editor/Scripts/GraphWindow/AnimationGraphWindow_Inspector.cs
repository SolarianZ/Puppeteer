using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private const float _INSPECTOR_PANEL_WIDTH = 300;

        private VisualElement _inspectorPanel;


        private void CreateInspectorPanel()
        {
            _inspectorPanel = new VisualElement
            {
                name = "inspector-panel",
                style =
                {
                    width = _INSPECTOR_PANEL_WIDTH,
                    height = Length.Percent(100),
                    borderLeftWidth = 1,
                    borderLeftColor = Color.black
                }
            };
            _layoutContainer.Add(_inspectorPanel);
        }
    }
}
