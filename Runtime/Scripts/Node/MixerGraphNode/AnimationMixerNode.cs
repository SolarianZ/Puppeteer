using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class AnimationMixerNode : NodeBase
    {
        #region Serialization Data

        public List<WeightedNodeInput> MixerInputs
        {
            get => _mixerInputs;
            internal set => _mixerInputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _mixerInputs = new List<WeightedNodeInput>();

        #endregion


        #region Runtime Properties

        private string[] _inputGuids;

        private ParamInfo[] _runtimeInputWeightParams;

        #endregion


        public AnimationMixerNode(string guid) : base(guid)
        {
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            if (Application.isPlaying)
            {
                _inputGuids ??= (from input in MixerInputs select input.InputNodeGuid).ToArray();
            }
            else
            {
                _inputGuids = (from input in MixerInputs select input.InputNodeGuid).ToArray();
            }

            return _inputGuids;
        }

        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            base.InitializeConnection(nodeGuidTable);

            for (int i = 0; i < MixerInputs.Count; i++)
            {
                Playable.SetInputWeight(i, GetLogicInputWeight(i));
            }
        }

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            // Input weights
            _runtimeInputWeightParams = new ParamInfo[MixerInputs.Count];
            for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
            {
                var index = i;
                var weightParam = MixerInputs[index].InputWeightParam;
                if (!weightParam.IsValue)
                {
                    var runtimeInputWeightParam = paramGuidTable[weightParam.Guid];
                    _runtimeInputWeightParams[index] = runtimeInputWeightParam;

                    runtimeInputWeightParam.OnValueChanged += p => OnInputWeightChanged(index, p.GetFloat());
                }
            }
        }

        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            var playable = AnimationMixerPlayable.Create(playableGraph, MixerInputs.Count);
            return playable;
        }

        protected override float GetLogicInputWeight(int inputIndex)
        {
            var runtimeInputWeightParam = _runtimeInputWeightParams[inputIndex];
            if (runtimeInputWeightParam != null)
            {
                return runtimeInputWeightParam.GetFloat();
            }

            return MixerInputs[inputIndex].InputWeightParam.GetFloat();
        }


        // TODO: OnInputWeightChanged
        private void OnInputWeightChanged(int index, float weight) => throw new NotImplementedException();
    }
}
