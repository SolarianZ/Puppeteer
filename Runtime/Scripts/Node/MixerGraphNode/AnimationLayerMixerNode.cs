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
    public class AnimationLayerMixerNode : NodeBase
    {
        #region Serialization Data

        // TODO: Should use LayeredNodeInput here!
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


        public AnimationLayerMixerNode(string guid) : base(guid)
        {
        }

        protected internal override IList<string> GetInputNodeGuids()
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

            var layerMixerPlayable = (AnimationLayerMixerPlayable)Playable;
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                var layeredNodeInput = (LayeredNodeInput)MixerInputs[i];
                layerMixerPlayable.SetInputWeight(i, GetLogicInputWeight(i));
                layerMixerPlayable.SetLayerAdditive((uint)i, layeredNodeInput.IsAdditive);
                if (layeredNodeInput.AvatarMask)
                {
                    layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)i, layeredNodeInput.AvatarMask);
                }
            }
        }

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(float deltaTime) => throw new NotImplementedException();


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
            var playable = AnimationLayerMixerPlayable.Create(playableGraph, MixerInputs.Count, MixerInputs.Count > 1);
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
