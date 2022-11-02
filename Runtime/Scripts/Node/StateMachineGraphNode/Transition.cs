using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    public enum TransitionMode
    {
        Smooth,

        Frozen
    }

    [Serializable]
    public class Transition
    {
        #region Serialization Data

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
        private AnimationCurve _blendCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public List<TransitionCondition> Conditions => _conditions;

        [SerializeField]
        private List<TransitionCondition> _conditions = new List<TransitionCondition>();

        // TODO: Interrupt?

        #endregion


        public Transition(string destStateGuid)
        {
            _destStateGuid = destStateGuid;
        }
    }
}
