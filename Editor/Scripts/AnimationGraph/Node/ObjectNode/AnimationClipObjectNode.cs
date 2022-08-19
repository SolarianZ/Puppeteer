using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationClipObjectNode : ObjectNode<AnimationClip, Playable>
    {
        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationClipObjectNode() : this(new ObjectNodeData
        {
            Title = "Animation Clip",
            Guid = NewGuid(),
            TypeAssemblyQualifiedName = typeof(AnimationClipObjectNode).AssemblyQualifiedName,
        })
        {
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationClipObjectNode(ObjectNodeData nodeData)
            : base(nodeData, "Clip", Colors.AnimationPlayableColor)
        {
        }
    }
}