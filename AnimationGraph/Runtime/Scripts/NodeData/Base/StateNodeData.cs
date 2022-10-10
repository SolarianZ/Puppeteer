using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class StateNodeData : NodeDataBase
    {
        public string GraphGuid => Guid;

        // TODO: Use graph data guid?
        // public GraphData.GraphData GraphData
        // {
        //     get => _graphData;
        //     internal set => _graphData = value;
        // }
        //
        // [SerializeField]
        // private GraphData.GraphData _graphData;

        // public string StateName
        // {
        //     get => GraphData.Name;
        //     internal set => GraphData.Name = value;
        // }

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();


        public StateNodeData(string graphGuid) : base(graphGuid)
        {
        }

        // public StateNodeData(GraphData.GraphData graphData) : base(graphData.Guid)
        // {
        //     GraphData = graphData;
        // }
    }
}
