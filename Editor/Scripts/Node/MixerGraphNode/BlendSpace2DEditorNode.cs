using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class BlendSpace2DEditorNode : MixerGraphEditorNode
    {
        internal new BlendSpace2DNode Node => (BlendSpace2DNode)base.Node;

        private BlendSpace2DNodeInspector _inspector;

        public BlendSpace2DEditorNode(AnimationGraphAsset graphAsset, BlendSpace2DNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Blend Space 2D";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new BlendSpace2DNodeInspector(AddInputPortElement,
                RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTarget(this);

            return _inspector;
        }
    }
}
