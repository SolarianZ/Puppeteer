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
    public class GraphLayer : IDisposable
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
        public List<NodeBase> IsolatedNodes => _isolatedNodes;

        [SerializeReference]
        private List<NodeBase> _isolatedNodes = new List<NodeBase>();

#endif

        #endregion


        #region Runtime Properties

        internal NodeBase RuntimeRootNode { get; private set; }

        internal IReadOnlyDictionary<string, NodeBase> NodeGuidTable => _nodeGuidTable;

        private Dictionary<string, NodeBase> _nodeGuidTable;

        #endregion


        public GraphLayer(string guid, string name, GraphType type)
        {
            _guid = guid;
            _name = name;
            _graphType = type;
        }

        public void InitializeNodes(PlayableGraph playableGraph,
            IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            _nodeGuidTable = new Dictionary<string, NodeBase>(Nodes.Count);
            foreach (var node in Nodes)
            {
                node.InitializeData(playableGraph, graphGuidTable, paramGuidTable);
                _nodeGuidTable.Add(node.Guid, node);

                if (RuntimeRootNode == null && node.Guid.Equals(RootNodeGuid))
                {
                    RuntimeRootNode = node;
                }
            }
        }

        public void InitializeConnections()
        {
            foreach (var node in Nodes)
            {
                node.InitializeConnection(NodeGuidTable);
            }
        }

        public void Dispose()
        {
            foreach (var node in _nodeGuidTable.Values)
            {
                node.Destroy();
            }

            _nodeGuidTable.Clear();
        }
    }
}
