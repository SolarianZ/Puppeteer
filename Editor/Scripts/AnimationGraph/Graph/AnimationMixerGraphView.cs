namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationMixerGraphView : AnimationGraphView
    {
        public AnimationMixerGraphView(AnimationGraphAsset asset)
            : base(asset)
        {
            if (Asset.EditorNodes.Count == 0)
            {
                CreateRootNode();
            }
        }


        public override AnimationGraphNode RootNode => _rootNode;

        private AnimationMixerNode _rootNode;

        private void CreateRootNode()
        {
            _rootNode = new AnimationMixerNode(true);
            AddElement(_rootNode);
        }
    }
}