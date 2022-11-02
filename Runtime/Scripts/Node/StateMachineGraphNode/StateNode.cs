using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateNode : NodeBase
    {
        #region Serialization Data

        // public string StateName;

        public List<Transition> Transitions => _transitions;

        [SerializeField]
        private List<Transition> _transitions = new List<Transition>();

        #endregion


        #region Runtime Properties

        public string LinkedGraphGuid => Guid;

        #endregion


        public StateNode(string graphGuid) : base(graphGuid)
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
