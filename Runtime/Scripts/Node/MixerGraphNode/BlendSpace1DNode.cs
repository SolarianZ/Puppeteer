﻿using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace1DInput : NodeInputBase
    {
        [SerializeField]
        public float Position;

        [SerializeField]
        public float PlaybackSpeed = 1.0f;
    }

    // TODO: Add Speed Param & Only Accept AnimationClip as Inputs
    [Serializable]
    public class BlendSpace1DNode : PlayableNodeBase
    {
        public ParamGuidOrValue PositionParam
        {
            get => _positionParam;
            internal set => _positionParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace1DInput> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace1DInput> _samples = new List<BlendSpace1DInput>();

        private string[] _inputGuids;


        public BlendSpace1DNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            if (Application.isPlaying)
            {
                _inputGuids ??= (from input in Samples select input.InputNodeGuid).ToArray();
            }
            else
            {
                _inputGuids = (from input in Samples select input.InputNodeGuid).ToArray();
            }

            return _inputGuids;
        }
    }
}