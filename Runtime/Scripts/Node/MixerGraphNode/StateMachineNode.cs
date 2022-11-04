using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateMachineNode : NodeBase
    {
        #region Serialization Data

        #endregion


        #region Runtime Properties

        public string StateMachineGraphGuid => Guid;


        private GraphLayer _stateMachineGraph;

        private string[] _inputGuids;

        private Dictionary<string, int> _stateGuidToIndexTable;

        #endregion


        public StateMachineNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeGraphLink(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, AnimationGraphAsset> externalGraphGuidTable)
        {
            base.InitializeGraphLink(graphGuidTable, externalGraphGuidTable);

            _stateMachineGraph = graphGuidTable[StateMachineGraphGuid];

            var stateCount = _stateMachineGraph.Nodes.Count;
            _inputGuids = new string[stateCount];
            _stateGuidToIndexTable = new Dictionary<string, int>(stateCount);
            for (int i = 0; i < stateCount; i++)
            {
                var stateNode = (StateNode)_stateMachineGraph.Nodes[i];
                _inputGuids[i] = stateNode.Guid;
                _stateGuidToIndexTable.Add(stateNode.Guid, i);
            }
        }

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
        }

        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            var playable = AnimationMixerPlayable.Create(playableGraph, _stateMachineGraph.Nodes.Count);
            return playable;
        }


        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            // Linked to external graph, so here we use node guid table of linked graph
            base.InitializeConnection(_stateMachineGraph?.NodeGuidTable);

            // Activate entry state
            if (!string.IsNullOrEmpty(_stateMachineGraph!.RootNodeGuid))
            {
                Playable.SetInputWeight(_stateGuidToIndexTable[_stateMachineGraph.RootNodeGuid], 1);
            }
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            _inputGuids ??= EmptyInputs; // Editor only

            return _inputGuids;
        }


        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        #endregion
    }
}
