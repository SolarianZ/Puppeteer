using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateMachineNode : MixerNodeBase
    {
        #region Serialization Data

        #endregion


        #region Runtime Properties

        public string StateMachineGraphGuid => Guid;

        #endregion


        public StateMachineNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
