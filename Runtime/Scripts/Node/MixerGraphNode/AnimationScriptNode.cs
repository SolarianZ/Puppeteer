using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract string Tag { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Animator animator, Skeleton skeleton);
    }

    [Serializable]
    public class AnimationScriptNode : MixerNodeBase
    {
        #region Mixer Inputs

        public List<WeightedNodeInput> MixerInputs
        {
            get => _mixerInputs;
            internal set => _mixerInputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _mixerInputs = new List<WeightedNodeInput>();

        private string[] _inputGuids;

        #endregion


        public AnimationScriptAsset ScriptAsset
        {
            get => _scriptAsset;
            internal set => _scriptAsset = value;
        }

        [SerializeField]
        private AnimationScriptAsset _scriptAsset;


        public AnimationScriptNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
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
    }
}
