using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class PlayableNodeBase : NodeBase
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

        #endregion


        #region Non Serializable Input Data

        protected static string[] EmptyInputs = Array.Empty<string>();

        public abstract IList<string> GetInputNodeGuids();

        #endregion
        

        #region Runtime Properties

        // internal abstract Playable AnimationPlayable { get; }

        #endregion


        protected PlayableNodeBase(string guid) : base(guid)
        {
        }


    }
}
