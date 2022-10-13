using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationScriptNode : PlayableNode
    {
        public AnimationScriptNode(AnimationGraphAsset graphAsset, AnimationScriptNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Animation Script";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new AnimationScriptNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
