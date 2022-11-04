using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    // https://docs.unity3d.com/ScriptReference/Animations.AnimatorState.html
    [Serializable]
    public class AnimationClipNode : AnimationAssetPlayerNodeBase
    {
        #region Serialization Data

        public AnimationClip Clip
        {
            get => _animationClip;
            internal set => _animationClip = value;
        }

        [SerializeField]
        private AnimationClip _animationClip;


        // public bool CycleOffsetParamActive
        // {
        //     get => _cycleOffsetParamActive;
        //     internal set => _cycleOffsetParamActive = value;
        // }
        //
        // [SerializeField]
        // private bool _cycleOffsetParamActive = false;
        //
        //
        // public ParamGuidOrValue CycleOffsetParam
        // {
        //     get => _cycleOffsetParam;
        //     internal set => _cycleOffsetParam = value;
        // }
        //
        // [SerializeField]
        // private ParamGuidOrValue _cycleOffsetParam = new ParamGuidOrValue(null, 0f);


        public bool ApplyFootIK
        {
            get => _applyFootIK;
            internal set => _applyFootIK = value;
        }

        [SerializeField]
        private bool _applyFootIK;


        public bool ApplyPlayableIK
        {
            get => _applyPlayableIK;
            internal set => _applyPlayableIK = value;
        }

        [SerializeField]
        private bool _applyPlayableIK;

        #endregion


        #region Runtime Properties

        public override float BaseSpeed
        {
            get
            {
                if (!SpeedParamActive) return 1;
                return _runtimeSpeedParam?.GetFloat() ?? SpeedParam.GetFloat();
            }
        }

        public override FrameData FrameData { get; protected set; }


        protected override float MotionTime
        {
            get
            {
                if (!MotionTimeParamActive) return 0;
                return _runtimeMotionTimeParam?.GetFloat() ?? MotionTimeParam.GetFloat();
            }
        }


        private ParamInfo _runtimeSpeedParam;

        private bool _runtimeSpeedDirty;

        private ParamInfo _runtimeMotionTimeParam;

        private bool _runtimeMotionTimeDirty;

        #endregion


        public AnimationClipNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            Assert.IsFalse(MotionTimeParamActive && SpeedParamActive,
                $"Use explicit motion time and speed at the same time. Node type: {GetType().Name}, node guid: {Guid}.");

            // Motion time
            if (MotionTimeParamActive)
            {
                if (!MotionTimeParam.IsLiteral)
                {
                    _runtimeMotionTimeParam = paramGuidTable[MotionTimeParam.Guid];
                    _runtimeMotionTimeParam.OnValueChanged += OnRuntimeMotionTimeParamChanged;
                }
            }

            // Speed
            if (SpeedParamActive)
            {
                if (!SpeedParam.IsLiteral)
                {
                    _runtimeSpeedParam = paramGuidTable[SpeedParam.Guid];
                    _runtimeSpeedParam.OnValueChanged += OnRuntimeSpeedParamChanged;
                }
            }

            _runtimeMotionTimeDirty = true;
            _runtimeSpeedDirty = true;
        }

        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            var playable = AnimationClipPlayable.Create(playableGraph, Clip);
            return playable;
        }


        protected internal override IReadOnlyList<string> GetInputNodeGuids() => EmptyInputs;


        protected internal override void PrepareFrame(FrameData frameData)
        {
            FrameData = frameData;

            if (_runtimeSpeedDirty)
            {
                if (SyncMethod == SyncMethod.DoNotSync)
                {
                    SetSpeed(BaseSpeed);
                }

                _runtimeSpeedDirty = false;
            }

            if (_runtimeMotionTimeDirty)
            {
                Playable.SetTime(MotionTime);
                _runtimeMotionTimeDirty = false;
            }
        }

        #endregion


        public override double GetUnscaledAnimationLength()
        {
            if (Clip) return Clip.length;
            return 0;
        }


        private void OnRuntimeSpeedParamChanged(ParamInfo paramInfo)
        {
            _runtimeSpeedDirty = true;
        }

        private void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo)
        {
            _runtimeMotionTimeDirty = true;
        }
    }
}
