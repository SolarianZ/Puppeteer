using GBG.AnimationGraph.Node;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEditorNode : MixerGraphEditorNode
    {
        public string StateMachineGraphGuid => ((StateMachineNode)Node).StateMachineGraphGuid;


        public StateMachineEditorNode(AnimationGraphAsset graphAsset, StateMachineNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "State Machine";

            RefreshPorts();
            RefreshExpandedState();
        }

        // Use default inspector
    }
}
