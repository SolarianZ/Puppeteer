using System;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class NodeInput
    {
        public string InputNodeGuid
        {
            get => _inputNodeGuid;
            internal set => _inputNodeGuid = value;
        }

        [SerializeField]
        private string _inputNodeGuid;
    }

    [Serializable]
    public class WeightedNodeInput : NodeInput
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
    public class LayeredNodeInput : WeightedNodeInput
    {
        public bool IsAdditive
        {
            get => _isAdditive;
            internal set => _isAdditive = value;
        }

        [SerializeField]
        private bool _isAdditive;

        public AvatarMask AvatarMask
        {
            get => _avatarMask;
            internal set => _avatarMask = value;
        }

        [SerializeField]
        private AvatarMask _avatarMask;
    }
}
