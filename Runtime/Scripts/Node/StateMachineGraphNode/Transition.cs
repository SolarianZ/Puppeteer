using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using GBG.AnimationGraph.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Node
{
    public enum TransitionMode
    {
        Smooth = 0,

        Frozen
    }

    [Serializable]
    public class Transition
    {
        #region Serialization Data

        public string SrcStateGuid => _srcStateGuid;

        [SerializeField]
        private string _srcStateGuid;

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
        [InspectorName("Exit Time(%)")] // of input clip which has the highest weight 
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
        private AnimationCurve _blendCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public List<TransitionCondition> Conditions => _conditions;

        [SerializeField]
        private List<TransitionCondition> _conditions = new();

        // TODO: Interrupt?

        #endregion


        #region Runtime Properties

        public Action OnMeetConditions;

        #endregion


        [Obsolete]
        public Transition(string destStateGuid)
        {
            _destStateGuid = destStateGuid;
        }

        public Transition(string srcStateGuid, string destStateGuid)
        {
            _srcStateGuid = srcStateGuid;
            _destStateGuid = destStateGuid;
        }

        public void Initialize(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            Assert.IsTrue(CurveTool.IsNormalized(BlendCurve), "Blend curve in animation transition is not normalized.");
            // $"Blend curve of animation transition '{FromNodeGuid} -> {DestNodeGuid}' is not normalized."

            foreach (var condition in Conditions)
            {
                condition.Initialize(paramGuidTable);
                condition.OnConditionStateChanged += OnConditionStateChanged;
            }
        }

        public bool CheckTransitions()
        {
            foreach (var condition in Conditions)
            {
                if (condition.Result)
                {
                    return true;
                }
            }

            return false;
        }


        private void OnConditionStateChanged(bool isMeetCondition)
        {
            if (isMeetCondition)
            {
                OnMeetConditions?.Invoke();
            }
        }
    }
}
