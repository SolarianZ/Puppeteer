namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationMixerGraphView : AnimationGraphView
    {
        public AnimationMixerGraphView()
        {

            CreateRootNode();
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