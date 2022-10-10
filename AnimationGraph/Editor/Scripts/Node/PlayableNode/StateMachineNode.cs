using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineNode : PlayableNode
    {
        public override string Guid => NodeData.Guid;

        public string StateMachineGraphGuid => ((StateMachineNodeData)NodeData).StateMachineGraphGuid;


        public StateMachineNode(AnimationGraphAsset graphAsset, StateMachineNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "State Machine";

            RefreshPorts();
            RefreshExpandedState();
        }

        // Use default inspector
    }
}
