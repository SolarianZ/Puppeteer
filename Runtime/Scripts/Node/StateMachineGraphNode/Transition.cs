using System;
using System.Collections.Generic;
using System.Diagnostics;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Node
{
    public enum ConditionOperator
    {
        Equals,

        NotEquals,

        Greater,

        Less,
    }

    [Serializable]
    public class TransitionCondition
    {
        public ParamType ParamType
        {
            get => _paramType;
            internal set => _paramType = value;
        }

        private ParamType _paramType;

        public ParamGuidOrValue LeftParam
        {
            get => _leftParam;
            set => _leftParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _leftParam = new ParamGuidOrValue(null, 0);

        public ParamGuidOrValue RightParam
        {
            get => _rightParam;
            set => _rightParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _rightParam = new ParamGuidOrValue(null, 0);

        public ConditionOperator Operator
        {
            get => _operator;
            set => _operator = value;
        }

        [SerializeField]
        private ConditionOperator _operator;


        [Conditional("UNITY_ASSERTIONS")]
        public void AssertCheck()
        {
            Assert.IsNotNull(LeftParam, "Left operand is null.");
            Assert.IsNotNull(RightParam, "Right operand is null.");
            // Assert.AreEqual(LeftParam.ParamType, ParamType,
            //     $"Left operand is not the type of {ParamType}.");
            // Assert.AreEqual(RightParam.ParamType, ParamType,
            //     $"Right operand is not the type of {ParamType}.");

            if (Operator == ConditionOperator.Greater || Operator == ConditionOperator.Less)
            {
                Assert.AreNotEqual(ParamType.Bool, ParamType,
                    $"Use {ConditionOperator.Greater} or {ConditionOperator.Less} on {ParamType.Bool} operands.");
            }
        }


        public static bool CheckCondition(ConditionOperator op, float left, float right)
        {
            switch (op)
            {
                case ConditionOperator.Greater:
                    return left > right;

                case ConditionOperator.Less:
                    return left < right;

                case ConditionOperator.Equals:
                    return Mathf.Approximately(left, right);

                case ConditionOperator.NotEquals:
                    return !Mathf.Approximately(left, right);

                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }
    }

    public enum TransitionMode
    {
        Smooth,

        Frozen
    }

    [Serializable]
    public class Transition
    {
        // public string Guid => _guid;
        //
        // [SerializeField]
        // private string _guid;

        public string DestStateGuid => _destStateGuid;

        [SerializeField]
        private string _destStateGuid;

        public float ExitTime
        {
            get => _exitTime;
            internal set => _exitTime = value;
        }

        [SerializeField]
        [Range(0f, 1f)]
        [InspectorName("Exit Time(%)")]
        private float _exitTime; // In percentage terms

        public float FadeTime
        {
            get => _fadeTime;
            internal set => _fadeTime = value;
        }

        [SerializeField]
        [Min(0f)]
        [InspectorName("Fade Time(s)")]
        private float _fadeTime;

        public TransitionMode TransitionMode
        {
            get => _transitionMode;
            internal set => _transitionMode = value;
        }

        [SerializeField]
        private TransitionMode _transitionMode = TransitionMode.Smooth;

        public AnimationCurve BlendCurve
        {
            get => _blendCurve;
            internal set => _blendCurve = value;
        }

        [SerializeField]
        private AnimationCurve _blendCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public List<TransitionCondition> Conditions => _conditions;

        [SerializeField]
        private List<TransitionCondition> _conditions = new List<TransitionCondition>();

        // TODO: Interrupt?


        public Transition(string destStateGuid)
        {
            _destStateGuid = destStateGuid;
        }
    }
}
