using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using GBG.AnimationGraph.Utility;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UObject = UnityEngine.Object;
using UPlayable = UnityEngine.Playables.Playable;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class AnimationScriptNode : NodeBase
    {
        #region Serialization Data

        public List<WeightedNodeInput> MixerInputs
        {
            get => _mixerInputs;
            internal set => _mixerInputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _mixerInputs = new List<WeightedNodeInput>();


        public AnimationScriptAsset ScriptAsset
        {
            get => _scriptAsset;
            internal set => _scriptAsset = value;
        }

        [SerializeField]
        private AnimationScriptAsset _scriptAsset;

        #endregion


        #region Runtime Properties

        public new AnimationScriptPlayable Playable => (AnimationScriptPlayable)base.Playable;


        private string[] _inputGuids;

        private ParamInfo[] _runtimeInputWeightParams;

        private float[] _runtimeInputWeights;

        private bool _isInputWeightDirty;

        private PrepareFrameArgs _prepareFrameArgs;

        #endregion


        public AnimationScriptNode(string guid) : base(guid)
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

            // Input weight of AnimationScriptPlayable input should be always 1,
            // process weight in AnimationJob.
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                Playable.SetInputWeight(i, 1);
            }
        }

        protected internal override void PrepareFrame(FrameData frameData)
        {
            if (!ScriptAsset)
            {
                return;
            }

            if (_isInputWeightDirty)
            {
                for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
                {
                    var weightParam = _runtimeInputWeightParams[i];
                    var weight = weightParam?.GetFloat() ?? MixerInputs[i].InputWeightParam.GetFloat();
                    _runtimeInputWeights[i] = weight;
                }

                if (ScriptAsset.NormalizeInputWeights)
                {
                    WeightTool.NormalizeWeights(_runtimeInputWeights, _runtimeInputWeights);
                }
            }

            _prepareFrameArgs ??= new PrepareFrameArgs(RuntimeInputNodes, _runtimeInputWeights);
            _prepareFrameArgs.IsInputWeightDirty = _isInputWeightDirty;
            ScriptAsset.PrepareFrame(Playable, frameData, _prepareFrameArgs);

            _isInputWeightDirty = false;
        }

        protected internal override void Dispose()
        {
            if (ScriptAsset)
            {
                ScriptAsset.Dispose();
                UObject.Destroy(ScriptAsset);
            }

            base.Dispose();
        }


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (ScriptAsset)
            {
                ScriptAsset = UObject.Instantiate(ScriptAsset);
            }

            // Input weights
            var inputCount = MixerInputs.Count;
            _runtimeInputWeights = new float[inputCount];
            _runtimeInputWeightParams = new ParamInfo[inputCount];
            for (int i = 0; i < inputCount; i++)
            {
                var weightParam = MixerInputs[i].InputWeightParam;
                if (!weightParam.IsValue)
                {
                    var runtimeInputWeightParam = paramGuidTable[weightParam.Guid];
                    _runtimeInputWeightParams[i] = runtimeInputWeightParam;

                    runtimeInputWeightParam.OnValueChanged += OnInputWeightChanged;
                }
            }

            _isInputWeightDirty = true;
        }

        // TODO: Need Skeleton argument.
        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            if (!ScriptAsset)
            {
                return UPlayable.Null;
            }

            var playable = ScriptAsset.CreatePlayable(null, playableGraph, MixerInputs.Count);
            Assert.AreEqual(playable.GetInputCount(), MixerInputs.Count,
                $"Runtime playable input count({playable.GetInputCount()}) doesn't equal to serialized input count({MixerInputs.Count}).");

            return playable;
        }


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

                if (ScriptAsset && ScriptAsset.NormalizeInputWeights)
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
