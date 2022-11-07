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

        [Obsolete]
        public AnimationGraphAsset SubGraph
        {
            get => _subGraph;
            internal set => _subGraph = value;
        }

        [Obsolete]
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


        private AnimationGraphAsset _linkedGraphAsset;

        private string[] _inputGuids;

        private ParamBinding[] _runtimeParamBindings;

        #endregion


        public SubGraphNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeGraphLink(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, AnimationGraphAsset> externalGraphGuidTable)
        {
            base.InitializeGraphLink(graphGuidTable, externalGraphGuidTable);

            _linkedGraphAsset = externalGraphGuidTable[LinkedGraphGuid];
            if (_linkedGraphAsset)
            {
                _inputGuids = _linkedGraphAsset.RuntimeRootGraph != null
                    ? new[] { _linkedGraphAsset.RuntimeRootGraph.RootNodeGuid }
                    : Array.Empty<string>();
            }
        }

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (!_linkedGraphAsset)
            {
                return;
            }

            // Binding params
            var paramBindingCount = ParamBindings.Count;
            _runtimeParamBindings = new ParamBinding[paramBindingCount];
            for (int i = 0; i < paramBindingCount; i++)
            {
                var bindingInfo = ParamBindings[i];
                var destParam = _linkedGraphAsset.FindParameterByGuid(bindingInfo.DestParamGuid);
                if (bindingInfo.IsLiteral())
                {
                    destParam.SetRawValue(bindingInfo.GetRawValue());
                }
                else
                {
                    var srcParam = paramGuidTable[bindingInfo.GetSrcParamGuid()];
                    var paramBinding = new ParamBinding(srcParam, destParam, true);
                    _runtimeParamBindings[i] = paramBinding;
                }
            }
        }

        protected override Playable CreatePlayable(Animator animator, PlayableGraph playableGraph)
        {
            // SubGraphNode has and only has one input
            var playable = AnimationMixerPlayable.Create(playableGraph, 1);
            return playable;
        }

        
        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            // Linked to external graph, so here we use node guid table of linked graph
            base.InitializeConnection(_linkedGraphAsset.RuntimeRootGraph?.NodeGuidTable);

            // Sub graph node has and only has one input
            Playable.SetInputWeight(0, 1);
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            _inputGuids ??= EmptyInputs; // Editor only

            return _inputGuids;
        }

        
        protected internal override void PrepareFrame(FrameData frameData)
        {
            if (_linkedGraphAsset)
            {
                _linkedGraphAsset.RuntimeRootGraph?.RuntimeRootNode.PrepareFrame(frameData);
            }
        }

        protected internal override void Dispose()
        {
            if (_runtimeParamBindings != null)
            {
                foreach (var paramBinding in _runtimeParamBindings)
                {
                    paramBinding.Dispose();
                }

                _runtimeParamBindings = null;
            }

            base.Dispose();
        }

        #endregion
    }
}
