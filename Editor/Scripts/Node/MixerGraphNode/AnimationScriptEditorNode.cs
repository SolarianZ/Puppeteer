using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationScriptEditorNode : MixerGraphEditorNode
    {
        public AnimationScriptEditorNode(AnimationGraphAsset graphAsset, AnimationScriptNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
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
