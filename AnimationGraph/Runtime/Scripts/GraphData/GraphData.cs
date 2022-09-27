using System;
using System.Collections.Generic;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.GraphData
{
    public enum GraphType
    {
        StateMachine = 0,

        Mixer = 1
    }

    [Serializable]
    public class GraphData
    {
        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        [SerializeField]
        private string _name;


        public string Guid => _guid;

        [SerializeField]
        private string _guid;


        public GraphType GraphType
        {
            get => _graphType;
            internal set => _graphType = value;
        }

        [SerializeField]
        private GraphType _graphType = GraphType.StateMachine;


        public string RootNodeGuid
        {
            get => _rootNodeGuid;
            internal set => _rootNodeGuid = value;
        }

        [SerializeField]
        private string _rootNodeGuid;


        public List<NodeDataBase> Nodes => _nodes;

        [SerializeReference]
        private List<NodeDataBase> _nodes = new List<NodeDataBase>();


        public GraphData(string guid, string name, GraphType type)
        {
            _guid = guid;
            _name = name;
            _graphType = type;
        }
    }
}
