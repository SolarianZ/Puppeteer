using System;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject, IDisposable
    {
        public abstract bool NormalizeInputWeights { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Skeleton skeleton,
            PlayableGraph playableGraph, int inputCount);

        // Called by ScriptNode
        public abstract void PrepareFrame(Playable playable, in FrameData info, PrepareFrameArgs args);

        public abstract void Dispose();


        #region Mono Messages

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        #endregion
    }
}
