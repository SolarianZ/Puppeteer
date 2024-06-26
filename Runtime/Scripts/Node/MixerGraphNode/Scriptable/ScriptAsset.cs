﻿using System;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class ScriptAsset : ScriptableObject, IDisposable
    {
        public abstract bool NormalizeInputWeights { get; }


        public abstract Playable CreateScriptPlayable(Animator animator,
            PlayableGraph playableGraph, int inputCount);

        public abstract ScriptBehaviour GetScriptBehaviour();

        public virtual void Dispose()
        {
        }


        #region Mono Messages

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        #endregion
    }

    public abstract class ScriptBehaviour : PlayableBehaviour, IDisposable
    {
        // Called by PlayableGraph
        public sealed override void PrepareFrame(Playable playable, UnityEngine.Playables.FrameData info)
        {
            base.PrepareFrame(playable, info);
        }

        // Called by ScriptNode
        public void PrepareFrame(Playable playable, in FrameData info, PrepareFrameArgs args)
        {
            BeforePrepareInputNodesFrame(playable, info, args);
            PrepareInputNodesFrame(playable, info, args);
            AfterPrepareInputNodesFrame(playable, info, args);
        }

        public virtual void Dispose()
        {
        }


        protected abstract void BeforePrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);

        protected abstract void PrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);

        protected abstract void AfterPrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);
    }
}
