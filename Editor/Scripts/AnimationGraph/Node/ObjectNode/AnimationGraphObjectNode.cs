using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationGraphObjectNode : ObjectNode<AnimationGraphAsset, Playable>
    {
        // for build contextual menu
        // ReSharper disable once UnusedMember.Global
        public AnimationGraphObjectNode() : this(new ObjectNodeData
        {
            Title = "Animation Graph",
            Guid = NewGuid(),
            TypeAssemblyQualifiedName = typeof(AnimationGraphObjectNode).AssemblyQualifiedName,
        })
        {
        }

        // for rebuild node
        // ReSharper disable once UnusedMember.Global
        public AnimationGraphObjectNode(ObjectNodeData nodeData)
            : base(nodeData, "Graph", Colors.AnimationPlayableColor)
        {
        }
    }
}