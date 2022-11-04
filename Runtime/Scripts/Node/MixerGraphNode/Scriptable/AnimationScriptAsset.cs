using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public abstract class AnimationScriptAsset : ScriptableObject
    {
        public abstract bool NormalizeInputWeights { get; }


        public abstract AnimationScriptPlayable CreatePlayable(Skeleton skeleton,
            PlayableGraph playableGraph, int inputCount);

        // Called by ScriptNode
        public void PrepareFrame(Playable playable, in FrameData info, PrepareFrameArgs args)
        {
            BeforePrepareInputNodesFrame(playable, info, args);
            PrepareInputNodesFrame(playable, info, args);
            AfterPrepareInputNodesFrame(playable, info, args);
        }


        protected abstract void BeforePrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);

        protected abstract void PrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);

        protected abstract void AfterPrepareInputNodesFrame(Playable playable,
            in FrameData info, PrepareFrameArgs args);
    }
}
