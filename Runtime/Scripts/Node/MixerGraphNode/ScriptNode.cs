using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using GBG.AnimationGraph.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Node
{
    // TODO: Support parameter binding?
    [Serializable]
    public class ScriptNode : NodeBase
    {
        #region Serialization Data

        public List<WeightedNodeInput> Inputs
        {
            get => _inputs;
            internal set => _inputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _inputs = new List<WeightedNodeInput>();


        public ScriptAsset Script
        {
            get => _script;
            internal set => _script = value;
        }

        [SerializeField]
        private ScriptAsset _script;

        #endregion


        #region Runtime Properties

        private ScriptBehaviour _scriptBehaviour;

        private string[] _inputGuids;

        private ParamInfo[] _runtimeInputWeightParams;

        private float[] _runtimeInputWeights;

        private bool _isInputWeightDirty = true;

        private PrepareFrameArgs _prepareFrameArgs;

        #endregion


        public ScriptNode(string guid) : base(guid)
        {
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids()
        {
            if (Application.isPlaying)
            {
                _inputGuids ??= (from input in Inputs select input.InputNodeGuid).ToArray();
            }
            else
            {
                _inputGuids = (from input in Inputs select input.InputNodeGuid).ToArray();
            }

            return _inputGuids;
        }

        protected internal override void PrepareFrame(FrameData frameData)
        {
            if (_scriptBehaviour == null)
            {
                return;
            }

            if (_isInputWeightDirty)
            {
                for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
                {
                    var weightParam = _runtimeInputWeightParams[i];
                    var weight = weightParam?.GetFloat() ?? Inputs[i].InputWeightParam.GetFloat();
                    _runtimeInputWeights[i] = weight;
                }

                if (Script.NormalizeInputWeights)
                {
                    WeightTool.NormalizeWeights(_runtimeInputWeights, _runtimeInputWeights);
                }
            }

            _prepareFrameArgs ??= new PrepareFrameArgs(RuntimeInputNodes, _runtimeInputWeights);
            _prepareFrameArgs.IsInputWeightDirty = _isInputWeightDirty;
            _scriptBehaviour.PrepareFrame(Playable, frameData, _prepareFrameArgs);

            _isInputWeightDirty = false;
        }

        protected internal override void Destroy()
        {
            if (Script)
            {
                _scriptBehaviour.Dispose();
                UObject.Destroy(Script);
            }


            base.Destroy();
        }


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (Script)
            {
                Script = UObject.Instantiate(Script);
            }

            // Input weights
            var inputCount = Inputs.Count;
            _runtimeInputWeights = new float[inputCount];
            _runtimeInputWeightParams = new ParamInfo[inputCount];
            for (int i = 0; i < inputCount; i++)
            {
                var weightParam = Inputs[i].InputWeightParam;
                if (!weightParam.IsValue)
                {
                    var runtimeInputWeightParam = paramGuidTable[weightParam.Guid];
                    _runtimeInputWeightParams[i] = runtimeInputWeightParam;

                    runtimeInputWeightParam.OnValueChanged += OnInputWeightChanged;
                }
            }
        }


        // TODO: Need GameObject argument.
        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            if (!Script)
            {
                return Playable.Null;
            }

            var playable = Script.CreateScriptPlayable(null, playableGraph, Inputs.Count);
            Assert.AreEqual(playable.GetInputCount(), Inputs.Count,
                $"Runtime playable input count({playable.GetInputCount()}) doesn't equal to serialized input count({Inputs.Count}).");

            _scriptBehaviour = Script.GetScriptBehaviour();

            return playable;
        }


        private float GetLogicInputWeight(int inputIndex)
        {
            if (_isInputWeightDirty)
            {
                for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
                {
                    var weightParam = _runtimeInputWeightParams[i];
                    var weight = weightParam?.GetFloat() ?? Inputs[i].InputWeightParam.GetFloat();
                    _runtimeInputWeights[i] = weight;
                }

                if (Script && Script.NormalizeInputWeights)
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
