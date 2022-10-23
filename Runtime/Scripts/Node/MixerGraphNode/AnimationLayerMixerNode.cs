using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class LayerMixerInputData : MixerInputData
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

    [Serializable]
    public class AnimationLayerMixerNode : PlayableNodeBase
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


        public AnimationLayerMixerNode(string guid) : base(guid)
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
