using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateNodeData : NodeDataBase
    {
        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        [SerializeField]
        private string _name;

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();

        public StateNodeData(string guid) : base(guid)
        {
        }
    }
}
