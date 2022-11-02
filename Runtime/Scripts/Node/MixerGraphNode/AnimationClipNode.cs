using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    // https://docs.unity3d.com/ScriptReference/Animations.AnimatorState.html
    [Serializable]
    public class AnimationClipNode : NodeBase
    {
        #region Serialization Data

        public AnimationClip Clip
        {
            get => _animationClip;
            internal set => _animationClip = value;
        }

        [SerializeField]
        private AnimationClip _animationClip;


        public bool SpeedParamActive
        {
            get => _speedParamActive;
            internal set => _speedParamActive = value;
        }

        [SerializeField]
        private bool _speedParamActive;

        public ParamGuidOrValue SpeedParam
        {
            get => _speedParam;
            internal set => _speedParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _speedParam = new ParamGuidOrValue(null, 1.0f);


        public bool MotionTimeParamActive
        {
            get => _motionTimeParamActive;
            internal set => _motionTimeParamActive = value;
        }

        [SerializeField]
        private bool _motionTimeParamActive;


        public ParamGuidOrValue MotionTimeParam
        {
            get => _motionTimeParam;
            internal set => _motionTimeParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _motionTimeParam = new ParamGuidOrValue(null, 0f);


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

        public float BaseSpeed { get; private set; }


        private ParamInfo _runtimeSpeedParam;

        private ParamInfo _runtimeMotionTimeParam;

        #endregion


        public AnimationClipNode(string guid) : base(guid)
        {
        }

        protected internal override IList<string> GetInputNodeGuids() => EmptyInputs;

        protected internal override void PrepareFrame(float deltaTime) => throw new NotImplementedException();

        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            if (!SpeedParamActive)
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

        protected override Playable CreatePlayable(PlayableGraph playableGraph) => throw new NotImplementedException();

        protected override float GetInputWeight(int inputIndex) => throw new InvalidOperationException();


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
