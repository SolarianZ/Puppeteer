using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private Toolbar _toolbar;

        private void CreateToolbar()
        {
            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            // Ping asset button
            var pingAssetButton = new Button(PingGraphAsset)
            {
                text = "Ping Asset"
            };
            _toolbar.Add(pingAssetButton);

            // Save asset button
            var saveAssetButton = new Button(SaveChanges)
            {
                text = "Save Asset"
            };
            _toolbar.Add(saveAssetButton);

            // Frame all button
            var frameAllButton = new Button(() => { _graphView.FrameAll(); })
            {
                text = "Frame All"
            };
            _toolbar.Add(frameAllButton);

            // Toggle blackboard button
            var toggleBlackboardButton = new Button(ToggleBlackboardPanel)
            {
                text = "Toggle Blackboard"
            };
            _toolbar.Add(toggleBlackboardButton);

            // Toggle inspector button
            var toggleInspectorButton = new Button(ToggleInspectorPanel)
            {
                text = "Toggle Inspector"
            };
            _toolbar.Add(toggleInspectorButton);
        }


        private void PingGraphAsset()
        {
            EditorGUIUtility.PingObject(_graphAsset);
        }

        private void ToggleBlackboardPanel()
        {
            var showPanel = !_blackboardPanel.enabledSelf;
            _blackboardPanel.SetEnabled(showPanel);
            _blackboardPanel.style.width = showPanel ? _BLACKBOARD_PANEL_WIDTH : 0;
        }

        private void ToggleInspectorPanel()
        {
            var showPanel = !_inspectorPanel.enabledSelf;
            _inspectorPanel.SetEnabled(showPanel);
            _inspectorPanel.style.width = showPanel ? _INSPECTOR_PANEL_WIDTH : 0;
        }
    }
}
