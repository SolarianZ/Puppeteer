using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class PlayableNodeData : NodeDataBase
    {
        public bool SpeedParamActive
        {
            get => _speedParamActive;
            internal set => _speedParamActive = value;
        }

        [SerializeField]
        private bool _speedParamActive;

        public ParamGuidOrValue SpeedParam
        {
            get => _speedParam;
            internal set => _speedParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _speedParam = new ParamGuidOrValue(null, 1.0f);

        public List<MixerInputData> MixerInputs
        {
            get => _mixerInputs;
            internal set => _mixerInputs = value;
        }

        [SerializeReference]
        private List<MixerInputData> _mixerInputs = new List<MixerInputData>();


        public PlayableNodeData(string guid) : base(guid)
        {
        }
    }
}
