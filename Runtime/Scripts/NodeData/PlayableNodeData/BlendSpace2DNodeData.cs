using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class BlendSpace2DSample
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

    [Serializable]
    public class BlendSpace2DNodeData : PlayableNodeData
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

        public List<BlendSpace2DSample> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace2DSample> _samples = new List<BlendSpace2DSample>();

        public int[] Triangles
        {
            get => _triangles;
            internal set => _triangles = value;
        }

        [SerializeField]
        private int[] _triangles = Array.Empty<int>();


        public BlendSpace2DNodeData(string guid) : base(guid)
        {
        }
    }
}
