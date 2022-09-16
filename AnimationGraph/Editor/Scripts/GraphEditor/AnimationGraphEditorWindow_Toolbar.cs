using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private Toolbar _toolbar;

        private void CreateToolbar()
        {
            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            // Ping asset button
            var pingAssetButton = new ToolbarButton(PingGraphAsset)
            {
                text = "Ping Asset"
            };
            _toolbar.Add(pingAssetButton);

            // Save asset button
            var saveAssetButton = new ToolbarButton(SaveChanges)
            {
                text = "Save Asset"
            };
            _toolbar.Add(saveAssetButton);

            // Frame all button
            var frameAllButton = new ToolbarButton(() => { ActiveGraphView.FrameAll(); })
            {
                text = "Frame All"
            };
            _toolbar.Add(frameAllButton);

            // Blackboard toggle
            var blackboardToggle = new ToolbarToggle
            {
                text = "Blackboard",
                value = true,
            };
            blackboardToggle.RegisterValueChangedCallback(ToggleBlackboard);
            _toolbar.Add(blackboardToggle);

            // Inspector toggle
            var inspectorToggle = new ToolbarToggle
            {
                text = "Inspector",
                value = true,
            };
            inspectorToggle.RegisterValueChangedCallback(ToggleInspector);
            _toolbar.Add(inspectorToggle);
        }


        private void PingGraphAsset()
        {
            EditorGUIUtility.PingObject(_graphAsset);
        }

        private void ToggleBlackboard(ChangeEvent<bool> evt)
        {
            _layoutContainer.ToggleLeftPane(evt.newValue);
        }

        private void ToggleInspector(ChangeEvent<bool> evt)
        {
            _layoutContainer.ToggleRightPane(evt.newValue);
        }
    }
}
