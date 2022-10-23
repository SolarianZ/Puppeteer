using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationClipEditorNode : MixerGraphEditorNode
    {
        public AnimationClipEditorNode(AnimationGraphAsset graphAsset, AnimationClipNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
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
