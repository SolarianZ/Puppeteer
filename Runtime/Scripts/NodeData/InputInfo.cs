using System;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class InputInfo : ICloneable
    {
        public string InputNodeGuid => _inputNodeGuid;

        [SerializeField]
        private string _inputNodeGuid;


        public object Clone()
        {
            var clone = CreateCloneInstance();
            CloneMembers(clone);

            return clone;
        }

        protected virtual void CloneMembers(InputInfo clone)
        {
            clone._inputNodeGuid = _inputNodeGuid;
        }

        protected virtual InputInfo CreateCloneInstance()
        {
            return new InputInfo();
        }
    }

    [Serializable]
    public class MixerInputInfo : InputInfo
    {
        public ParamNameOrValue InputWeightParam => _inputWeightParam;

        [SerializeField]
        private ParamNameOrValue _inputWeightParam = new ParamNameOrValue(null, 0);


        protected override void CloneMembers(InputInfo clone)
        {
            base.CloneMembers(clone);

            var mixerClone = (MixerInputInfo)clone;
            mixerClone._inputWeightParam = _inputWeightParam;
        }

        protected override InputInfo CreateCloneInstance()
        {
            return new MixerInputInfo();
        }
    }

    [Serializable]
    public class LayerMixerInputInfo : MixerInputInfo
    {
        public bool IsAdditive => _isAdditive;

        [SerializeField]
        private bool _isAdditive;


        public AvatarMask AvatarMask => _avatarMask;

        [SerializeField]
        private AvatarMask _avatarMask;


        protected override void CloneMembers(InputInfo clone)
        {
            base.CloneMembers(clone);

            var layerMixerClone = (LayerMixerInputInfo)clone;
            layerMixerClone._isAdditive = _isAdditive;
            layerMixerClone._avatarMask = _avatarMask;
        }

        protected override InputInfo CreateCloneInstance()
        {
            return new LayerMixerInputInfo();
        }
    }
}
