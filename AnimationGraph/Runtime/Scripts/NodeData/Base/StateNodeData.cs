using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateNodeData : NodeDataBase
    {
        public string GraphGuid => Guid;

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();


        public StateNodeData(string graphGuid) : base(graphGuid)
        {
        }
    }
}
