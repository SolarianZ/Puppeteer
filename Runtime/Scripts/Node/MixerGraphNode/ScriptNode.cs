using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class ScriptAsset : ScriptableObject
    {
        public abstract Playable CreateScriptPlayable(GameObject gameObject,
            PlayableGraph playableGraph, int inputCount);

        public abstract ScriptBehaviour GetScriptBehaviour();
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

        private ParamInfo[] _runtimeInputWeightParams;

        private ScriptBehaviour _scriptBehaviour;

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

        protected internal override void InitializeConnection(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            base.InitializeConnection(graphGuidTable, nodeGuidTable);

            for (int i = 0; i < Inputs.Count; i++)
            {
                Playable.SetInputWeight(i, GetInputWeight(i));
            }
        }

        protected internal override void PrepareFrame(float deltaTime) => throw new NotImplementedException();


        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            // Input weights
            _runtimeInputWeightParams = new ParamInfo[Inputs.Count];
            for (int i = 0; i < _runtimeInputWeightParams.Length; i++)
            {
                var index = i;
                var weightParam = Inputs[index].InputWeightParam;
                if (!weightParam.IsValue)
                {
                    var runtimeInputWeightParam = paramGuidTable[weightParam.Guid];
                    _runtimeInputWeightParams[index] = runtimeInputWeightParam;

                    runtimeInputWeightParam.OnValueChanged += p => OnInputWeightChanged(index, p.GetFloat());
                }
            }
        }


        protected override Playable CreatePlayable(PlayableGraph playableGraph)
        {
            if (!Script)
            {
                return Playable.Null;
            }

            // TODO: Need GameObject argument.
            var playable = Script.CreateScriptPlayable(null, playableGraph, Inputs.Count);
            _scriptBehaviour = Script.GetScriptBehaviour();

            return playable;
        }

        protected override float GetInputWeight(int inputIndex)
        {
            var runtimeInputWeightParam = _runtimeInputWeightParams[inputIndex];
            if (runtimeInputWeightParam != null)
            {
                return runtimeInputWeightParam.GetFloat();
            }

            return Inputs[inputIndex].InputWeightParam.GetFloat();
        }


        // TODO: OnInputWeightChanged
        private void OnInputWeightChanged(int index, float weight) => throw new NotImplementedException();
    }
}
