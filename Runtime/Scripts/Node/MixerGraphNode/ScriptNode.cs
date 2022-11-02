using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Component;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class ScriptAsset : ScriptableObject
    {
        public abstract string Tag { get; }


        public abstract Playable CreateScriptPlayable<T>(Animator animator, Skeleton skeleton)
            where T : ScriptBehaviour;
    }

    public abstract class ScriptBehaviour : PlayableBehaviour
    {
    }

    // TODO: Parameter binding?
    [Serializable]
    public class ScriptNode : NodeBase
    {
        #region Serialization Data

        public List<WeightedNodeInput> Inputs
        {
            get => _inputs;
            internal set => _inputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _inputs = new List<WeightedNodeInput>();


        public ScriptAsset Script
        {
            get => _script;
            internal set => _script = value;
        }

        [SerializeField]
        private ScriptAsset _script;

        #endregion


        #region Runtime Properties

        private string[] _inputGuids;

        #endregion


        public ScriptNode(string guid) : base(guid)
        {
        }

        protected internal override IList<string> GetInputNodeGuids()
        {
            if (Application.isPlaying)
            {
                _inputGuids ??= (from input in Inputs select input.InputNodeGuid).ToArray();
            }
            else
            {
                _inputGuids = (from input in Inputs select input.InputNodeGuid).ToArray();
            }

            return _inputGuids;
        }
        
        protected internal override void PrepareFrame(float deltaTime)=> throw new NotImplementedException();

        
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)=> throw new NotImplementedException();

        protected override Playable CreatePlayable(PlayableGraph playableGraph)=> throw new NotImplementedException();

        protected override float GetInputWeight(int inputIndex)=> throw new NotImplementedException();
    }
}
