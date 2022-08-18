namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationMixerNode : AnimationMixerNodeBase
    {
        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationMixerNode() : this(false) { }

        public AnimationMixerNode(bool isRootNode) : base(new NodeData
        {
            Guid = NewGuid(),
            IsRootNode = isRootNode,
            TypeAssemblyQualifiedName = typeof(AnimationMixerNode).AssemblyQualifiedName,
        }, false)
        {
            // title
            title = "Animation Mixer";
            NodeData.Title = title;
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationMixerNode(NodeData nodeData) : base(nodeData, true)
        {
            // title
            title = "Animation Mixer";
            NodeData.Title = title;
        }
    }
}