using System;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class InputInfo : ICloneable
    {
        public string InputNodeGuid
        {
            get => _inputNodeGuid;
            set => _inputNodeGuid = value;
        }

        [SerializeField]
        private string _inputNodeGuid;


        public InputInfo(string inputNodeGuid)
        {
            _inputNodeGuid = inputNodeGuid;
        }


        #region Deep Clone

        protected InputInfo()
        {
        }

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

        #endregion
    }

    [Serializable]
    public class MixerInputInfo : InputInfo
    {
        public ParamNameOrValue InputWeightParam
        {
            get => _inputWeightParam;
            set => _inputWeightParam = value;
        }

        [SerializeField]
        private ParamNameOrValue _inputWeightParam = new ParamNameOrValue(null, 0);


        public MixerInputInfo(string inputNodeGuid, ParamNameOrValue inputWeightParam)
            : base(inputNodeGuid)
        {
            _inputWeightParam = inputWeightParam;
        }


        #region Deep Clone

        protected MixerInputInfo()
        {
        }

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

        #endregion
    }

    [Serializable]
    public class LayerMixerInputInfo : MixerInputInfo
    {
        public bool IsAdditive
        {
            get => _isAdditive;
            set => _isAdditive = value;
        }

        [SerializeField]
        private bool _isAdditive;


        public AvatarMask AvatarMask
        {
            get => _avatarMask;
            set => _avatarMask = value;
        }

        [SerializeField]
        private AvatarMask _avatarMask;


        public LayerMixerInputInfo(string inputNodeGuid, ParamNameOrValue inputWeightParam,
            bool isAdditive, AvatarMask avatarMask)
            : base(inputNodeGuid, inputWeightParam)
        {
            _isAdditive = isAdditive;
            _avatarMask = avatarMask;
        }

        #region Deep Clone

        protected LayerMixerInputInfo()
        {
        }

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

        #endregion
    }
}
