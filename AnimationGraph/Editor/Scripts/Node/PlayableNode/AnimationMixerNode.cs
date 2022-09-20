using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationMixerNode : PlayableNode
    {
        public AnimationMixerNode(AnimationGraphAsset graphAsset, AnimationMixerNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Mixer";

            RefreshPorts();
            RefreshExpandedState();
        }


        // public override GraphNodeInspector GetInspector()
        // {
        //     var inspector = new AnimationMixerNodeInspector(GraphAsset.Parameters);
        //     inspector.SetTargetNode(this);
        //
        //     return inspector;
        // }
    }
}
