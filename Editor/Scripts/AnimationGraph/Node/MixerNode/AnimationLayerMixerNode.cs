using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationLayerMixerNode : AnimationMixerNodeBase
    {
        public bool IsAdditive { get; }

        public AvatarMask AvatarMask { get; }


        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationLayerMixerNode() : this(false) { }

        public AnimationLayerMixerNode(bool isRootNode) : base(new NodeData
        {
            Guid = NewGuid(),
            IsRootNode = isRootNode,
            TypeAssemblyQualifiedName = typeof(AnimationLayerMixerNode).AssemblyQualifiedName,
        }, false)
        {
            title = "Animation Layer Mixer";
            NodeData.Title = title;
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationLayerMixerNode(NodeData nodeData) : base(nodeData, true)
        {
            title = "Animation Layer Mixer";
            NodeData.Title = title;
        }
    }
}