namespace GBG.AnimationGraph.NodeData
{
    public class StateMachineNodeData : PlayableNodeData
    {
        public string StateMachineGraphGuid => Guid;


        public StateMachineNodeData(string guid) : base(guid)
        {
        }
    }
}
