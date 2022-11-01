﻿using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace2DInput : NodeInputBase
    {
        [SerializeField]
        public AnimationClip Clip;

        [SerializeField]
        public Vector2 Position;

        [SerializeField]
        public float PlaybackSpeed = 1.0f;

        // [SerializeField]
        // public bool Mirror;
    }

    // TODO: Add Speed Param & Only Accept AnimationClip as Inputs
    [Serializable]
    public class BlendSpace2DNode : PlayableNodeBase
    {
        public ParamGuidOrValue PositionXParam
        {
            get => _positionXParam;
            internal set => _positionXParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionXParam = new ParamGuidOrValue(null, 0);

        public ParamGuidOrValue PositionYParam
        {
            get => _positionYParam;
            internal set => _positionYParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionYParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace2DInput> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace2DInput> _samples = new List<BlendSpace2DInput>();

        public int[] Triangles
        {
            get => _triangles;
            internal set => _triangles = value;
        }

        [SerializeField]
        private int[] _triangles = Array.Empty<int>();

        private string[] _inputGuids;


        public BlendSpace2DNode(string guid) : base(guid)
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