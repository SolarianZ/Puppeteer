using System;
using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
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

        public virtual ParamInfo[] GetParameters()
        {
            return null;
        }

        public virtual void Dispose()
        {
        }
    }
}
