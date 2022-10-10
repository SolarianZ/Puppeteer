using System;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateMachineNodeData : PlayableNodeData
    {
        public string StateMachineGraphGuid => Guid;


        public StateMachineNodeData(string guid) : base(guid)
        {
        }
    }
}
