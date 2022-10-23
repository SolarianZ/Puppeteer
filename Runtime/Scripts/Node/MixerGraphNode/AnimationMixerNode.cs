using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class MixerInputData : NodeInputBase
    {
        public ParamGuidOrValue InputWeightParam
        {
            get => _inputWeightParam;
            internal set => _inputWeightParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _inputWeightParam = new ParamGuidOrValue(null, 0);
    }
    
    [Serializable]
    public class AnimationMixerNode : PlayableNodeBase
    {
        #region Mixer Inputs

        public List<MixerInputData> MixerInputs
        {
            get => _mixerInputs;
            internal set => _mixerInputs = value;
        }

        [SerializeReference]
        private List<MixerInputData> _mixerInputs = new List<MixerInputData>();

        private string[] _inputGuids;

        #endregion


        public AnimationMixerNode(string guid) : base(guid)
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
