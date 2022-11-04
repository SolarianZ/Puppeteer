using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class SubGraphNode : NodeBase
    {
        #region Serialization Data

        public AnimationGraphAsset SubGraph
        {
            get => _subGraph;
            internal set => _subGraph = value;
        }

        [SerializeField]
        private AnimationGraphAsset _subGraph;

        public List<ParamBindingGuidOrValue> ParamBindings
        {
            get => _paramBindings;
            internal set => _paramBindings = value;
        }

        [SerializeField]
        private List<ParamBindingGuidOrValue> _paramBindings = new List<ParamBindingGuidOrValue>();

        #endregion


        #region Runtime Properties

        public string LinkedGraphGuid => Guid;


        private GraphLayer _linkedGraph;

        private string[] _inputGuids;

        #endregion


        public SubGraphNode(string guid) : base(guid)
        {
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }

        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            base.InitializeConnection(nodeGuidTable);

            // SubGraphNode has and only has one input
            Playable.SetInputWeight(0, 1);
        }

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        protected internal override void Destroy()
        {
            if (SubGraph)
            {
                UObject.Destroy(SubGraph);
            }

            base.Destroy();
        }


        // TODO: InitializeParams
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (SubGraph)
            {
                SubGraph = UObject.Instantiate(SubGraph);
            }


            throw new NotImplementedException();
        }

        // TODO: CreatePlayable
        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            // SubGraphNode has and only has one input
            var playable = AnimationMixerPlayable.Create(playableGraph, 1);
            return playable;
        }
    }
}
