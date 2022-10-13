using System;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;

namespace GBG.AnimationGraph.NodeData
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract string Tag { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Animator animator, Skeleton skeleton);
    }

    [Serializable]
    public class AnimationScriptNodeData : PlayableNodeData
    {
        public AnimationScriptAsset ScriptAsset
        {
            get => _scriptAsset;
            internal set => _scriptAsset = value;
        }

        [SerializeField]
        private AnimationScriptAsset _scriptAsset;


        public AnimationScriptNodeData(string guid) : base(guid)
        {
        }
    }
}
