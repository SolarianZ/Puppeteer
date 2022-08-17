namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationLayerMixerGraphView : AnimationGraphView
    {
        public AnimationLayerMixerGraphView()
        {
            CreateRootNode();
        }


        public override AnimationGraphNode RootNode => _rootNode;

        private AnimationLayerMixerNode _rootNode;

        private void CreateRootNode()
        {
            _rootNode = new AnimationLayerMixerNode(true);
            AddElement(_rootNode);
        }
    }
}