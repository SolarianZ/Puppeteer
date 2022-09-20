using System;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class MixerInputData
    {
        public string InputNodeGuid
        {
            get => _inputNodeGuid;
            internal set => _inputNodeGuid = value;
        }

        [SerializeField]
        private string _inputNodeGuid;


        public ParamGuidOrValue InputWeightParam
        {
            get => _inputWeightParam;
            internal set => _inputWeightParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _inputWeightParam;
    }
}
