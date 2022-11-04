using System;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class AnimationAssetPlayerNodeBase : NodeBase
    {
        #region Serialization Data

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


        public SyncMethod SyncMethod
        {
            get => _syncMethod;
            internal set => _syncMethod = value;
        }

        [SerializeField]
        private SyncMethod _syncMethod = SyncMethod.DoNotSync;

        public string SyncGroupName
        {
            get => _syncGroupName;
            internal set => _syncGroupName = value;
        }

        [SerializeField]
        private string _syncGroupName;

        public SyncGroupRole SyncGroupRole
        {
            get => _syncGroupRole;
            internal set => _syncGroupRole = value;
        }

        [SerializeField]
        private SyncGroupRole _syncGroupRole = SyncGroupRole.CanBeLeader;

        #endregion


        #region Runtime Properties

        public abstract float BaseSpeed { get; }

        public abstract FrameData FrameData { get; protected set; }


        protected abstract float MotionTime { get; }

        #endregion


        protected AnimationAssetPlayerNodeBase(string guid) : base(guid)
        {
        }


        public double GetScaledAnimationLength()
        {
            return GetUnscaledAnimationLength() / BaseSpeed;
        }

        public abstract double GetUnscaledAnimationLength();
    }
}
