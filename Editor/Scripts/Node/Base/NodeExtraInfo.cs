namespace GBG.AnimationGraph.Editor.Node
{
    public class NodeExtraInfo
    {
        public bool IsCreateFromContextualMenu { get; }


        public NodeExtraInfo(bool isCreateFromContextualMenu)
        {
            IsCreateFromContextualMenu = isCreateFromContextualMenu;
        }
    }
}
