using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using GBG.AnimationGraph.Utility;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class AnimationLayerMixerNode : NodeBase
    {
        #region Serialization Data

        public bool NormalizeInputWeights
        {
            get => _normalizeInputWeights;
            internal set => _normalizeInputWeights = value;
        }

        [SerializeField]
        private bool _normalizeInputWeights;


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

        private float[] _runtimeInputWeights;

        private bool _isInputWeightDirty;

        #endregion


        public AnimationLayerMixerNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            // Input weights
            var inputCount = MixerInputs.Count;
            _runtimeInputWeights = new float[inputCount];
            _runtimeInputWeightParams = new ParamInfo[inputCount];
            for (int i = 0; i < inputCount; i++)
            {
                var weightParam = MixerInputs[i].InputWeightParam;
                if (!weightParam.IsLiteral)
                {
                    var runtimeInputWeightParam = paramGuidTable[weightParam.Guid];
                    _runtimeInputWeightParams[i] = runtimeInputWeightParam;

                    runtimeInputWeightParam.OnValueChanged += OnInputWeightChanged;
                }
            }

            _isInputWeightDirty = true;
        }

        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            var playable = AnimationLayerMixerPlayable.Create(playableGraph, MixerInputs.Count, MixerInputs.Count > 1);
            return playable;
        }

        
        protected internal override void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            base.InitializeConnection(nodeGuidTable);

            var layerMixerPlayable = (AnimationLayerMixerPlayable)Playable;
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                var layeredNodeInput = (LayeredNodeInput)MixerInputs[i];
                layerMixerPlayable.SetLayerAdditive((uint)i, layeredNodeInput.IsAdditive);
                if (layeredNodeInput.AvatarMask)
                {
                    layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)i, layeredNodeInput.AvatarMask);
                }
            }
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


        protected internal override void PrepareFrame(FrameData frameData)
        {
            for (int i = 0; i < RuntimeInputNodes.Length; i++)
            {
                var inputWeight = GetLogicInputWeight(i);
                if (_isInputWeightDirty) Playable.SetInputWeight(i, inputWeight);

                var inputNode = RuntimeInputNodes[i];
                inputNode?.PrepareFrame(new FrameData(frameData, inputWeight));
            }

            _isInputWeightDirty = false;
        }

        #endregion


        private float GetLogicInputWeight(int inputIndex)
        {
            if (_isInputWeightDirty)
            {
                for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
                {
                    var weightParam = _runtimeInputWeightParams[i];
                    var weight = weightParam?.GetFloat() ?? MixerInputs[i].InputWeightParam.GetFloat();
                    _runtimeInputWeights[i] = weight;
                }

                if (NormalizeInputWeights)
                {
                    WeightTool.NormalizeWeights(_runtimeInputWeights, _runtimeInputWeights);
                }
            }

            return _runtimeInputWeights[inputIndex];
        }

        private void OnInputWeightChanged(ParamInfo paramInfo)
        {
            _isInputWeightDirty = true;
        }
    }
}
