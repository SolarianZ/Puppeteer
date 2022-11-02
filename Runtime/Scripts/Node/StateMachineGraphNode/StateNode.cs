using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateNode : NodeBase
    {
        #region Serialization Data

        // public string StateName;

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();

        #endregion


        #region Runtime Properties

        public string LinkedGraphGuid => Guid;

        #endregion


        public StateNode(string graphGuid) : base(graphGuid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
