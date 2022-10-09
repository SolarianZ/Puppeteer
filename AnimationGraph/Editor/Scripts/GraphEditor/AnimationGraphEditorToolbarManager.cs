using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public class AnimationGraphEditorToolbarManager
    {
        public event Action OnWantsToPingAsset;

        public event Action OnWantsToSaveChanges;

        public event Action OnWantsToFrameAll;

        public event Action<bool> OnWantsToToggleBlackboard;

        public event Action<bool> OnWantsToToggleInspector;

        // TODO: Show live debug target Animator

        public AnimationGraphEditorToolbarManager(AnimationGraphEditorMode mode, VisualElement parent)
        {
            var toolbar = new Toolbar();
            parent.Add(toolbar);

            // Mode
            var modeLabel = new Label(mode.ToString())
            {
                style =
                {
                    marginLeft = 3,
                    marginRight = 3,
                }
            };
            modeLabel.SetEnabled(false);
            toolbar.Add(modeLabel);

            // Ping asset button
            var pingAssetButton = new ToolbarButton(PingAsset)
            {
                text = "Ping Asset"
            };
            toolbar.Add(pingAssetButton);

            // Save asset button
            var saveAssetButton = new ToolbarButton(SaveChanges)
            {
                text = "Save Changes"
            };
            toolbar.Add(saveAssetButton);

            // Frame all button
            var frameAllButton = new ToolbarButton(FrameAll)
            {
                text = "Frame All"
            };
            toolbar.Add(frameAllButton);

            // Blackboard toggle
            var blackboardToggle = new ToolbarToggle
            {
                text = "Blackboard",
                value = true,
            };
            blackboardToggle.RegisterValueChangedCallback(evt => ToggleBlackboard(evt.newValue));
            toolbar.Add(blackboardToggle);

            // Inspector toggle
            var inspectorToggle = new ToolbarToggle
            {
                text = "Inspector",
                value = true,
            };
            inspectorToggle.RegisterValueChangedCallback(evt => ToggleInspector(evt.newValue));
            toolbar.Add(inspectorToggle);
        }


        private void PingAsset()
        {
            OnWantsToPingAsset?.Invoke();
        }

        private void SaveChanges()
        {
            OnWantsToSaveChanges?.Invoke();
        }

        private void FrameAll()
        {
            OnWantsToFrameAll?.Invoke();
        }

        private void ToggleBlackboard(bool enable)
        {
            OnWantsToToggleBlackboard?.Invoke(enable);
        }

        private void ToggleInspector(bool enable)
        {
            OnWantsToToggleInspector?.Invoke(enable);
        }
    }
}
