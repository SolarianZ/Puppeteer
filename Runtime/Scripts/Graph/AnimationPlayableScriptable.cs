using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Graph
{
    public abstract class AnimationScriptableObject : ScriptableObject, IDisposable
    {
        public abstract AnimationScriptPlayable CreatePlayable(PlayableGraph graph, Skeleton skeleton);

        public virtual void PrepareFrame(float deltaTime)
        {
        }

        public virtual void ProcessFrame(float deltaTime)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}