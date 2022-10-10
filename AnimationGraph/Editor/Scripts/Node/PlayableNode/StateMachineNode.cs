using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    // TODO: StateMachineNode
    public sealed class StateMachineNode : PlayableNode
    {
        public StateMachineNode(AnimationGraphAsset graphAsset, StateMachineNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "State Machine";
            
            RefreshPorts();
            RefreshExpandedState();
        }
        
        // TODO: Inspector
    }
}
