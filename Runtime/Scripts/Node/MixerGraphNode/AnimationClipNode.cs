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

        public override FrameData FrameData { get; protected set; }


        private bool _runtimeSpeedDirty;

        private bool _runtimeMotionTimeDirty;

        #endregion


        public AnimationClipNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        protected override void InitializeAssetPlayerParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            _runtimeMotionTimeDirty = true;
            _runtimeSpeedDirty = true;
        }

        protected override Playable CreatePlayable(Animator animator, PlayableGraph playableGraph)
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


        protected override void OnRuntimeSpeedParamChanged(ParamInfo paramInfo)
        {
            _runtimeSpeedDirty = true;
        }

        protected override void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo)
        {
            _runtimeMotionTimeDirty = true;
        }
    }
}
