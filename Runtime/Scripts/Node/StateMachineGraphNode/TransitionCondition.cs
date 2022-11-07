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
    public class TransitionCondition : IDisposable
    {
        #region Serialization Data

        public ParamType ParamType
        {
            get => _paramType;
            internal set => _paramType = value;
        }

        [SerializeField]
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

        #endregion


        #region Runtime Properties

        public bool Result { get; set; }

        public Action<bool> OnConditionStateChanged;


        private ParamInfo _runtimeLeftParam;

        private ParamInfo _runtimeRightParam;

        #endregion


        public void Initialize(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            AssertCheck();

            if (!LeftParam.IsLiteral)
            {
                _runtimeLeftParam = paramGuidTable[LeftParam.Guid];
                _runtimeLeftParam.OnValueChanged += OnConditionParamChanged;
            }

            if (!RightParam.IsLiteral)
            {
                _runtimeRightParam = paramGuidTable[RightParam.Guid];
                _runtimeLeftParam.OnValueChanged += OnConditionParamChanged;
            }

            Result = Evaluate();
        }

        private void OnConditionParamChanged(ParamInfo animParam)
        {
            if (Result != Evaluate())
            {
                Result = !Result;
                OnConditionStateChanged?.Invoke(Result);
            }
        }


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

        public bool Evaluate()
        {
            var leftParamValue = _runtimeLeftParam?.RawValue ?? LeftParam.RawValue;
            var rightParamValue = _runtimeRightParam?.RawValue ?? RightParam.RawValue;
            return CheckCondition(Operator, leftParamValue, rightParamValue);
        }

        public void Dispose()
        {
            if (_runtimeLeftParam != null)
            {
                _runtimeLeftParam.OnValueChanged -= OnConditionParamChanged;
            }

            if (_runtimeRightParam != null)
            {
                _runtimeRightParam.OnValueChanged -= OnConditionParamChanged;
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
}
