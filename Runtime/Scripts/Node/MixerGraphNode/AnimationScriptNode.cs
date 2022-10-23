using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract string Tag { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Animator animator, Skeleton skeleton);
    }

    [Serializable]
    public class AnimationScriptNode : PlayableNodeBase
    {
        public AnimationScriptAsset ScriptAsset
        {
            get => _scriptAsset;
            internal set => _scriptAsset = value;
        }

        [SerializeField]
        private AnimationScriptAsset _scriptAsset;


        public AnimationScriptNode(string guid) : base(guid)
        {
        }

        public override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
    }
}
