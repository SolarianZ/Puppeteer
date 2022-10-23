using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEditorNode : MixerGraphEditorNode
    {
        public string StateMachineGraphGuid => ((StateMachineNodeData)NodeData).StateMachineGraphGuid;


        public StateMachineEditorNode(AnimationGraphAsset graphAsset, StateMachineNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "State Machine";

            RefreshPorts();
            RefreshExpandedState();
        }

        // Use default inspector
    }
}
