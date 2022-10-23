using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateNode : NodeBase
    {
        public string MixerGraphGuid => Guid;

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();


        public StateNode(string graphGuid) : base(graphGuid)
        {
        }
    }
}
