using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class SubGraphNode : NodeBase
    {
        #region Serialization Data

        public AnimationGraphAsset SubGraph
        {
            get => _subGraph;
            internal set => _subGraph = value;
        }

        [SerializeField]
        private AnimationGraphAsset _subGraph;

        public List<ParamBindingGuidOrValue> ParamBindings
        {
            get => _paramBindings;
            internal set => _paramBindings = value;
        }

        [SerializeField]
        private List<ParamBindingGuidOrValue> _paramBindings = new List<ParamBindingGuidOrValue>();

        #endregion


        public SubGraphNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
