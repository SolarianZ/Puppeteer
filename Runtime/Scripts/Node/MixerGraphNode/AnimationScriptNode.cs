using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Component;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract AnimationScriptPlayable CreatePlayable(Skeleton skeleton,
            PlayableGraph playableGraph, int inputCount);
    }

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

        private string[] _inputGuids;

        private ParamInfo[] _runtimeInputWeightParams;

        #endregion


        public AnimationScriptNode(string guid) : base(guid)
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

        protected internal override void InitializeConnection(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            base.InitializeConnection(graphGuidTable, nodeGuidTable);

            // TODO FIXME: Input weight of AnimationScriptPlayable input should be always 1! Process weight in AnimationJob!
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                Playable.SetInputWeight(i, GetInputWeight(i));
            }
        }

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
            if (!ScriptAsset)
            {
                return Playable.Null;
            }

            // TODO: Need Skeleton argument.
            var playable = ScriptAsset.CreatePlayable(null, playableGraph, MixerInputs.Count);
            return playable;
        }

        protected override float GetInputWeight(int inputIndex)
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
