using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

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

        protected internal override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
        
        // TODO: PrepareFrame
        protected internal override void PrepareFrame(float deltaTime)=> throw new NotImplementedException();

        
        // TODO: InitializeParams
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)=> throw new NotImplementedException();

        // TODO: CreatePlayable
        protected override Playable CreatePlayable(PlayableGraph playableGraph)=> throw new NotImplementedException();

        // TODO: GetInputWeight
        protected override float GetInputWeight(int inputIndex)=> throw new NotImplementedException();
    }
}
