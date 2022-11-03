using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
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

        private ParamInfo _runtimeSpeedParam;

        private ParamInfo _runtimeMotionTimeParam;

        #endregion


        public AnimationClipNode(string guid) : base(guid)
        {
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids() => EmptyInputs;

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            // Motion time
            if (MotionTimeParamActive)
            {
                if (!MotionTimeParam.IsValue)
                {
                    _runtimeMotionTimeParam = paramGuidTable[MotionTimeParam.Guid];
                    _runtimeMotionTimeParam.OnValueChanged += OnRuntimeMotionTimeParamChanged;
                }

                BaseSpeed = 0;
            }
            // Speed
            else if (!SpeedParamActive)
            {
                BaseSpeed = 1;
            }
            else if (SpeedParam.IsValue)
            {
                BaseSpeed = SpeedParam.GetFloat();
            }
            else
            {
                _runtimeSpeedParam = paramGuidTable[SpeedParam.Guid];
                _runtimeSpeedParam.OnValueChanged += OnRuntimeSpeedParamChanged;
                BaseSpeed = _runtimeSpeedParam.GetFloat();
            }
        }

        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            var playable = AnimationClipPlayable.Create(playableGraph, Clip);

            // Motion time
            if (MotionTimeParamActive)
            {
                if (MotionTimeParam.IsValue)
                {
                    playable.SetTime(MotionTimeParam.GetFloat());
                }
                else
                {
                    playable.SetTime(_runtimeMotionTimeParam.GetFloat());
                }
            }

            // Speed
            playable.SetSpeed(BaseSpeed);

            return playable;
        }

        protected override float GetLogicInputWeight(int inputIndex) => throw new InvalidOperationException();


        private void OnRuntimeSpeedParamChanged(ParamInfo paramInfo)
        {
            BaseSpeed = _runtimeSpeedParam.GetFloat();

            // TODO: Update Animation Speed
            // TODO: Update Sync Group
        }

        private void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo)
        {
            Playable.SetTime(paramInfo.GetFloat());
        }
    }
}
