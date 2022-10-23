namespace GBG.AnimationGraph.Editor.Node
{
    public class EditorNodeExtraInfo
    {
        public bool IsCreateFromContextualMenu { get; }


        public EditorNodeExtraInfo(bool isCreateFromContextualMenu)
        {
            IsCreateFromContextualMenu = isCreateFromContextualMenu;
        }
    }
}
