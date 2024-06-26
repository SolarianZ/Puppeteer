﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    // TODO: There should not be a playable node. Move Transitions to StateMachineNode.
    [Serializable]
    public class StateNode : NodeBase
    {
        #region Serialization Data

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();

        #endregion


        #region Runtime Properties

        public string LinkedGraphGuid => Guid;


        private GraphLayer _linkedGraph;

        private string[] _inputGuids;

        #endregion


        public StateNode(string graphGuid) : base(graphGuid)
        {
        }


        #region Lifecycle

        protected override void InitializeGraphLink(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, AnimationGraphAsset> externalGraphGuidTable)
        {
            base.InitializeGraphLink(graphGuidTable, externalGraphGuidTable);

            _linkedGraph = graphGuidTable[LinkedGraphGuid];
            _inputGuids = new[] { _linkedGraph.RootNodeGuid };
        }

        // TODO: InitializeParams(Transitions)
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            foreach (var transition in Transitions)
            {
                transition.Initialize(paramGuidTable);
                transition.OnMeetConditions += OnMeetConditions;
            }
        }

        protected override Playable CreatePlayable(Animator animator, PlayableGraph playableGraph)
        {
            // State node has and only has one input
            var playable = AnimationMixerPlayable.Create(playableGraph, 1);
            return playable;
        }


        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            // Linked to external graph, so here we use node guid table of linked graph
            base.InitializeConnection(_linkedGraph.NodeGuidTable);

            // State node has and only has one input
            Playable.SetInputWeight(0, 1);
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            _inputGuids ??= EmptyInputs; // Editor only

            return _inputGuids;
        }


        // TODO: PrepareFrame(Exit Time)
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        #endregion


        private void OnMeetConditions()
        {
            throw new NotImplementedException();
        }
    }
}
