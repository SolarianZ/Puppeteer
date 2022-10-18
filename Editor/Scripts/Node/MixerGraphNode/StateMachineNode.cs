using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineNode : MixerGraphNode
    {
        public string StateMachineGraphGuid => ((StateMachineNodeData)NodeData).StateMachineGraphGuid;


        public StateMachineNode(AnimationGraphAsset graphAsset, StateMachineNodeData nodeData,
            NodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "State Machine";

            RefreshPorts();
            RefreshExpandedState();
        }

        // Use default inspector
    }
}
