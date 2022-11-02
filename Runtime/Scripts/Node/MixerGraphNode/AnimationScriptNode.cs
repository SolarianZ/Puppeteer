using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Component;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract string Tag { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Animator animator, Skeleton skeleton);
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

        protected internal override void PrepareFrame(float deltaTime) => throw new NotImplementedException();


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable) =>
            throw new NotImplementedException();

        protected override Playable CreatePlayable(PlayableGraph playableGraph) => throw new NotImplementedException();

        protected override float GetInputWeight(int inputIndex) => throw new NotImplementedException();
    }
}
