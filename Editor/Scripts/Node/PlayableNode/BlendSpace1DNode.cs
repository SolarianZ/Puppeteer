using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class BlendSpace1DNode : PlayableNode
    {
        private BlendSpace1DNodeInspector _inspector;


        public BlendSpace1DNode(AnimationGraphAsset graphAsset, BlendSpace1DNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Blend Space 1D";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            _inspector ??= new BlendSpace1DNodeInspector();
            _inspector.SetTarget(this);

            return _inspector;
        }
    }
}
