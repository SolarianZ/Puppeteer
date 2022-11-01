﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class AnimationMixerNode : PlayableNodeBase
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