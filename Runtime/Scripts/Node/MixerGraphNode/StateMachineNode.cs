using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateMachineNode : PlayableNodeBase
    {
        public string StateMachineGraphGuid => Guid;


        public StateMachineNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
