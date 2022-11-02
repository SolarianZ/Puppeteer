using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class StateMachineNode : NodeBase
    {
        #region Serialization Data

        #endregion


        #region Runtime Properties

        public string StateMachineGraphGuid => Guid;

        #endregion


        public StateMachineNode(string guid) : base(guid)
        {
        }

        protected internal override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }

        protected internal override void PrepareFrame(float deltaTime) => throw new NotImplementedException();


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable) =>
            throw new NotImplementedException();

        protected override Playable CreatePlayable(PlayableGraph playableGraph) => throw new NotImplementedException();

        protected override float GetInputWeight(int inputIndex) => throw new NotImplementedException();
    }
}
