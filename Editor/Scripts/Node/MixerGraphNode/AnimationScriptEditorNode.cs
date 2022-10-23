using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationScriptEditorNode : MixerGraphEditorNode
    {
        public AnimationScriptEditorNode(AnimationGraphAsset graphAsset, AnimationScriptNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Animation Script";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new AnimationScriptNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
