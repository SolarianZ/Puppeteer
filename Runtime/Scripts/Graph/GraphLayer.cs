using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Graph
{
    public enum GraphType
    {
        StateMachine = 0,

        Mixer = 1
    }

    [Serializable]
    public class GraphLayer
    {
        #region Serialization Data

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
        private GraphType _graphType;


        public string RootNodeGuid
        {
            get => _rootNodeGuid;
            internal set => _rootNodeGuid = value;
        }

        [SerializeField]
        private string _rootNodeGuid;


        public List<NodeBase> Nodes => _nodes;

        [SerializeReference]
        private List<NodeBase> _nodes = new List<NodeBase>();

#if UNITY_EDITOR
        public Vector2 EditorGraphRootNodePosition
        {
            get => _editorGraphRootNodePosition;
            internal set => _editorGraphRootNodePosition = value;
        }

        [SerializeField]
        private Vector2 _editorGraphRootNodePosition;

        // TODO: Isolated nodes(Editor only) 
#endif

        #endregion


        #region Runtime Properties

        #endregion


        public GraphLayer(string guid, string name, GraphType type)
        {
            _guid = guid;
            _name = name;
            _graphType = type;
        }

        public void InitializeNodes(PlayableGraph playableGraph,
            IReadOnlyDictionary<string, ParamInfo> paramGuidTable,
            Dictionary<string, NodeBase> outNodeGuidTable)
        {
            foreach (var node in Nodes)
            {
                node.InitializePlayable(playableGraph, paramGuidTable);
                outNodeGuidTable.Add(node.Guid, node);
            }
        }

        public void InitializeConnections(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            foreach (var node in Nodes)
            {
                node.InitializeConnection(nodeGuidTable);
            }
        }
    }
}
