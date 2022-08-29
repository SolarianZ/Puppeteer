using System;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public abstract class AnimationNodeInstance : IDisposable
    {
        public abstract Playable Playable { get; }


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