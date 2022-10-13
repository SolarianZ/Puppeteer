using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class BlendSpace2DNode : PlayableNode
    {
        private BlendSpace2DNodeInspector _inspector;

        public BlendSpace2DNode(AnimationGraphAsset graphAsset, BlendSpace2DNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Blend Space 2D";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            _inspector ??= new BlendSpace2DNodeInspector();
            _inspector.SetTarget(this);

            return _inspector;
        }
    }
}
