using System;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    // https://docs.unity3d.com/ScriptReference/Animations.AnimatorState.html
    [Serializable]
    public class AnimationClipNodeData : PlayableNodeData
    {
        public AnimationClip Clip
        {
            get => _animationClip;
            internal set => _animationClip = value;
        }

        [SerializeField]
        private AnimationClip _animationClip;


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


        public AnimationClipNodeData(string guid) : base(guid)
        {
        }
    }
}
