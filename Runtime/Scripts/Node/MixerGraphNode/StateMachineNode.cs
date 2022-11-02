using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateMachineNode : MixerNodeBase
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
