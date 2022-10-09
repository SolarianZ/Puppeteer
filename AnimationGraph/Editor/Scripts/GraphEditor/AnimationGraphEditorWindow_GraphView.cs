using UnityEngine;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        // Ensure this filed is serializable as thus we can keep this field's value after re-compile codes
        [SerializeField]
        [HideInInspector]
        private AnimationGraphEditorGraphViewManager _graphViewManager;


        private void CreateGraphViewPanel()
        {
            _graphViewManager = new AnimationGraphEditorGraphViewManager(_layoutContainer.MiddlePane);
            _graphViewManager.OnActiveGraphChanged += SetActiveGraph;
            _graphViewManager.OnGraphViewSelectionChanged += SetInspectTarget;
            _graphViewManager.OnDataChanged += OnDataChanged;
            _graphViewManager.OnWantsToSaveChanges += SaveChanges;
        }

        private void FrameAll()
        {
            _graphViewManager.FrameAll();
        }

        private void OpenGraphFromGraphList(string graphGuid)
        {
            _graphViewManager.OpenGraphView(_graphAsset, graphGuid, true);
        }
    }
}
