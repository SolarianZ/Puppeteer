using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class SubGraphEditorNode : MixerGraphEditorNode
    {
        public SubGraphEditorNode(AnimationGraphAsset graphAsset, SubGraphNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
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
