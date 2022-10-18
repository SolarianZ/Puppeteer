using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public abstract class PlayableNodeData : NodeDataBase
    {
        #region Speed

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


        protected PlayableNodeData(string guid) : base(guid)
        {
        }


        #region Non Serializable Input Data

        protected static string[] EmptyInputs = Array.Empty<string>();

        public abstract IList<string> GetInputNodeGuids();

        #endregion
    }
}
