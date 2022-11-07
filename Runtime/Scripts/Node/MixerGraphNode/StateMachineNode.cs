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

        private readonly Dictionary<string, List<Transition>> _stateGuidToTransitionTable = new();

        private readonly List<StateIndexAndInitialWeight> _transitionSources = new List<StateIndexAndInitialWeight>();

        private string _activeStateGuid;

        private Transition _activeTransition;

        private float _transitionTimer;

        private bool _isStateDirty;

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
            // Record transitions of each state
            foreach (var inputGuid in _inputGuids)
            {
                var inputNode = (StateNode)nodeGuidTable[inputGuid];
                foreach (var transition in inputNode.Transitions)
                {
                    if (!_stateGuidToTransitionTable.TryGetValue(inputGuid, out var transitionList))
                    {
                        transitionList = new();
                        _stateGuidToTransitionTable[inputGuid] = transitionList;
                    }

                    transitionList.Add(transition);
                }
            }

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
        protected internal override void PrepareFrame(FrameData frameData)
        {
            while (_isStateDirty)
            {
                var newActiveTransition = FindTargetTransition();
                if (newActiveTransition != null)
                {
                    RecordStatesBeforeTransitioning();
                    _activeStateGuid = newActiveTransition.DestStateGuid;
                    _transitionTimer = 0;
                    _activeTransition = newActiveTransition;
                }
                else
                {
                    _isStateDirty = false;
                }
            }

            if (_activeTransition != null)
            {
                _transitionTimer += frameData.DeltaTime;
                DoTransition();
            }
        }

        #endregion


        #region Transition

        private Transition FindTargetTransition()
        {
            var candidateTransitions = _stateGuidToTransitionTable[_activeStateGuid];
            Transition targetTransition = null;
            for (int i = 0; i < candidateTransitions.Count; i++)
            {
                var currTransition = candidateTransitions[i];
                if (!currTransition.CheckTransitions())
                {
                    continue;
                }

                targetTransition = currTransition;
                break;

                // if (targetTransition == null)
                // {
                //     targetTransition = currTransition;
                //     continue;
                // }

                // if (currTransition.PriorityOrder < targetTransition.PriorityOrder)
                // {
                //     targetTransition = currTransition;
                // }
            }

            return targetTransition;
        }

        private void RecordStatesBeforeTransitioning()
        {
            for (int i = 0; i < _transitionSources.Count; i++)
            {
                var oldSrcIndex = _transitionSources[i].Index;
                _transitionSources[i] =
                    new StateIndexAndInitialWeight(oldSrcIndex, Playable.GetInputWeight(oldSrcIndex));
            }

            var newSrcIndex = _stateGuidToIndexTable[_activeStateGuid];
            _transitionSources.Add(new StateIndexAndInitialWeight(newSrcIndex, Playable.GetInputWeight(newSrcIndex)));
        }

        private void DoTransition()
        {
            // Calculate weight
            var alpha = _activeTransition.BlendCurve.Evaluate(_transitionTimer / _activeTransition.FadeTime);
            var destInputIndex = _stateGuidToIndexTable[_activeTransition.DestStateGuid];
            Playable.SetInputWeight(destInputIndex, alpha);
            for (int i = 0; i < _transitionSources.Count; i++)
            {
                var srcState = _transitionSources[i];
                Playable.SetInputWeight(srcState.Index, srcState.InitialWeight * (1 - alpha));
            }

            // Transition completed
            if (_transitionTimer >= _activeTransition.FadeTime)
            {
                _transitionSources.Clear();
                _activeTransition = null;
            }
        }

        #endregion


        readonly struct StateIndexAndInitialWeight
        {
            public readonly int Index;

            public readonly float InitialWeight;

            public StateIndexAndInitialWeight(int index, float initialWeight)
            {
                Index = index;
                InitialWeight = initialWeight;
            }
        }
    }
}
