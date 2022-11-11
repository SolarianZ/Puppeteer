using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Assertions;

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

        public abstract FrameData FrameData { get; protected set; }

        public float BaseSpeed
        {
            get
            {
                if (!SpeedParamActive) return 1;
                return RuntimeSpeedParam?.GetFloat() ?? SpeedParam.GetFloat();
            }
        }


        protected float MotionTime
        {
            get
            {
                if (!MotionTimeParamActive) return 0;
                return RuntimeMotionTimeParam?.GetFloat() ?? MotionTimeParam.GetFloat();
            }
        }

        protected ParamInfo RuntimeSpeedParam { get; private set; }

        protected ParamInfo RuntimeMotionTimeParam { get; private set; }

        #endregion


        protected AnimationAssetPlayerNodeBase(string guid) : base(guid)
        {
        }

        protected sealed override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            Assert.IsFalse(MotionTimeParamActive && SpeedParamActive,
                $"Use explicit motion time and speed at the same time. Node type: {GetType().Name}, node guid: {Guid}.");

            // Motion time
            if (MotionTimeParamActive)
            {
                if (!MotionTimeParam.IsLiteral)
                {
                    RuntimeMotionTimeParam = paramGuidTable[MotionTimeParam.Guid];
                    RuntimeMotionTimeParam.OnValueChanged += OnRuntimeMotionTimeParamChanged;
                }
            }

            // Speed
            if (SpeedParamActive)
            {
                if (!SpeedParam.IsLiteral)
                {
                    RuntimeSpeedParam = paramGuidTable[SpeedParam.Guid];
                    RuntimeSpeedParam.OnValueChanged += OnRuntimeSpeedParamChanged;
                }
            }

            InitializeAssetPlayerParams(paramGuidTable);
        }

        protected abstract void InitializeAssetPlayerParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable);

        protected abstract void OnRuntimeSpeedParamChanged(ParamInfo paramInfo);

        protected abstract void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo);


        public double GetScaledAnimationLength()
        {
            return GetUnscaledAnimationLength() / BaseSpeed;
        }

        public abstract double GetUnscaledAnimationLength();
    }
}
