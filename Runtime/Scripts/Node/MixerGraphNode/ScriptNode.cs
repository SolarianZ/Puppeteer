using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Component;
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

    [Serializable]
    public class ScriptNode : MixerNodeBase
    {
        #region Mixer Inputs

        public List<WeightedNodeInput> Inputs
        {
            get => _inputs;
            internal set => _inputs = value;
        }

        [SerializeReference]
        private List<WeightedNodeInput> _inputs = new List<WeightedNodeInput>();

        private string[] _inputGuids;

        #endregion


        public ScriptAsset ScriptAsset
        {
            get => _scriptAsset;
            internal set => _scriptAsset = value;
        }

        [SerializeField]
        private ScriptAsset _scriptAsset;


        public ScriptNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
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
    }
}
