namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private AnimationGraphEditorBlackboardManager _blackboardManager;


        private void CreateBlackboardPanel()
        {
            _blackboardManager = new AnimationGraphEditorBlackboardManager(_editorMode, _layoutContainer.LeftPane);
            _blackboardManager.OnDataChanged += OnDataChanged;
            _blackboardManager.OnWantsOpenGraph += OpenGraphFromGraphList;
        }

        private void SetActiveGraph(int graphIndex)
        {
            _blackboardManager.SetActiveGraph(graphIndex);
        }
    }
}
