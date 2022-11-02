using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace1DSample
    {
        public AnimationClip Clip
        {
            get => _clip;
            internal set => _clip = value;
        }

        [SerializeField]
        private AnimationClip _clip;


        public float Position
        {
            get => _position;
            internal set => _position = value;
        }

        [SerializeField]
        private float _position;


        public float Speed
        {
            get => _speed;
            internal set => _speed = value;
        }

        [SerializeField]
        private float _speed = 1.0f;
    }

    [Serializable]
    public class BlendSpace1DNode : NodeBase
    {
        #region Serialization Data

        // public ParamGuidOrValue SpeedParam;

        public ParamGuidOrValue PositionParam
        {
            get => _positionParam;
            internal set => _positionParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace1DSample> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace1DSample> _samples = new List<BlendSpace1DSample>();

        #endregion


        public BlendSpace1DNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
