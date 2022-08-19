namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationMixerNode : AnimationMixerNodeBase
    {
        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationMixerNode() : this(false) { }

        public AnimationMixerNode(bool isRootNode) : base(new NodeData
        {
            Title = "Animation Mixer",
            Guid = NewGuid(),
            IsRootNode = isRootNode,
            TypeAssemblyQualifiedName = typeof(AnimationMixerNode).AssemblyQualifiedName,
        }, false)
        {
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationMixerNode(NodeData nodeData) : base(nodeData, true)
        {
        }
    }
}