namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private AnimationGraphEditorGraphViewManager _graphViewManager;

        private string[] _openedGraphGuids;


        private void CreateGraphViewPanel()
        {
            _graphViewManager = new AnimationGraphEditorGraphViewManager(_editorMode, _layoutContainer.MiddlePane);
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
