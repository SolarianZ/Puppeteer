namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationLayerMixerNode : AnimationGraphNode
    {
        public override bool AllowMultiInput => true;


        public AnimationLayerMixerNode() : this(false)
        {
        }

        public AnimationLayerMixerNode(bool isRootNode) : base(isRootNode)
        {
        }
    }
}