using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationScriptObjectNode : ObjectNode<AnimationScriptPlayableAsset, Playable>
    {
        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationScriptObjectNode() : this(new ObjectNodeData
        {
            Title = "Animation Script Asset",
            Guid = NewGuid(),
            TypeAssemblyQualifiedName = typeof(AnimationScriptObjectNode).AssemblyQualifiedName,
        })
        {
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationScriptObjectNode(ObjectNodeData nodeData)
            : base(nodeData, "Script Asset", Colors.AnimationPlayableColor)
        {
        }
    }
}