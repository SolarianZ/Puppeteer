using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class SubGraphEditorNode : MixerGraphEditorNode
    {
        public SubGraphEditorNode(AnimationGraphAsset graphAsset, SubGraphNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Sub Graph";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new SubGraphNodeInspector(GraphAsset.Parameters);
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
