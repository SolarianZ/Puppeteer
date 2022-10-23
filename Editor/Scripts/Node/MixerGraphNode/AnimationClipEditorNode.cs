using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationClipEditorNode : MixerGraphEditorNode
    {
        public AnimationClipEditorNode(AnimationGraphAsset graphAsset, AnimationClipNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "Animation Clip";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new AnimationClipNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
