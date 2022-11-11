using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

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
    public class BlendSpace1DNode : AnimationAssetPlayerNodeBase
    {
        #region Serialization Data

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


        #region Runtime Properties

        public override FrameData FrameData { get; protected set; }


        private ParamInfo _runtimePositionParam;

        private bool _runtimePositionDirty;

        private bool _runtimeSpeedDirty;

        private bool _runtimeMotionTimeDirty;

        #endregion


        public BlendSpace1DNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeAssetPlayerParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (!PositionParam.IsLiteral)
            {
                _runtimePositionParam = paramGuidTable[PositionParam.Guid];
                _runtimePositionParam.OnValueChanged += OnRuntimePositionParamChanged;
            }

            _runtimePositionDirty = true;
        }

        protected override Playable CreatePlayable(Animator animator, PlayableGraph playableGraph)
        {
            var playable = AnimationMixerPlayable.Create(playableGraph, Samples.Count);
            return playable;
        }


        protected internal override IReadOnlyList<string> GetInputNodeGuids() => EmptyInputs;


        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        #endregion


        // TODO: GetUnscaledAnimationLength
        public override double GetUnscaledAnimationLength()
        {
            throw new NotImplementedException();
        }


        protected override void OnRuntimeSpeedParamChanged(ParamInfo paramInfo)
        {
            _runtimeSpeedDirty = true;
        }

        protected override void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo)
        {
            _runtimeMotionTimeDirty = true;
        }


        private void OnRuntimePositionParamChanged(ParamInfo paramInfo)
        {
            _runtimePositionDirty = true;
        }

        private float GetPosition()
        {
            return _runtimePositionParam?.GetFloat() ?? PositionParam.GetFloat();
        }
    }
}
