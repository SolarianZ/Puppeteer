using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class SubGraphNode : PlayableNode
    {
        public SubGraphNode(AnimationGraphAsset graphAsset, SubGraphNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Sub Graph";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new SubGraphNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
