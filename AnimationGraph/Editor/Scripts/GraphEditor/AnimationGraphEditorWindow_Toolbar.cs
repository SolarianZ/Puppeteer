using UnityEditor;
using UnityEditor.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private Toolbar _toolbar;

        private void CreateToolbar()
        {
            var toolbarMgr = new AnimationGraphEditorToolbarManager(AnimationGraphEditorMode.Editor, rootVisualElement);
            toolbarMgr.OnWantsToPingAsset += PingGraphAsset;
            toolbarMgr.OnWantsToSaveChanges += SaveChanges;
            toolbarMgr.OnWantsToFrameAll += FrameAll;
            toolbarMgr.OnWantsToToggleBlackboard += ToggleBlackboard;
            toolbarMgr.OnWantsToToggleInspector += ToggleInspector;
        }

        private void PingGraphAsset()
        {
            EditorGUIUtility.PingObject(_graphAsset);
        }

        private void ToggleBlackboard(bool enable)
        {
            _layoutContainer.ToggleLeftPane(enable);
        }

        private void ToggleInspector(bool enable)
        {
            _layoutContainer.ToggleRightPane(enable);
        }
    }
}
