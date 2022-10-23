using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateMachineNodeData : PlayableNodeData
    {
        public string StateMachineGraphGuid => Guid;


        public StateMachineNodeData(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
