using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationClipNode : MixerGraphNode
    {
        public AnimationClipNode(AnimationGraphAsset graphAsset, AnimationClipNodeData nodeData,
            NodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "Animation Clip";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new AnimationClipNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
