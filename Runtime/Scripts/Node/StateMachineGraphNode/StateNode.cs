using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateNode : NodeBase
    {
        #region Serialization Data

        public string StateName
        {
            get => _stateName;
            internal set => _stateName = value;
        }

        [SerializeField]
        private string _stateName;


        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();

        #endregion


        #region Runtime Properties

        public string MixerGraphGuid => Guid;

        #endregion


        public StateNode(string graphGuid) : base(graphGuid)
        {
        }
    }
}
