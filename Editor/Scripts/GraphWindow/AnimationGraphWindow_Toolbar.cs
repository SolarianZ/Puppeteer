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
            var isVisible = !_layoutContainer.LeftPane.enabledSelf;
            _layoutContainer.ToggleLeftPane(isVisible);
        }

        private void ToggleInspectorPanel()
        {
            var isVisible = !_layoutContainer.RightPane.enabledSelf;
            _layoutContainer.ToggleRightPane(isVisible);
        }
    }
}
