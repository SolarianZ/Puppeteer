using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class BlendSpace1DEditorNode : MixerGraphEditorNode
    {
        internal new BlendSpace1DNode Node => (BlendSpace1DNode)base.Node;

        private BlendSpace1DNodeInspector _inspector;


        public BlendSpace1DEditorNode(AnimationGraphAsset graphAsset, BlendSpace1DNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Blend Space 1D";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new BlendSpace1DNodeInspector(AddInputPortElement,
                RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTarget(this);

            return _inspector;
        }
    }
}
