using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateNodeData : NodeDataBase
    {
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

        public StateNodeData(string guid) : base(guid)
        {
        }
    }
}
