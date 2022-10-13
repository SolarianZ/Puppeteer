using System;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Graph
{
    public class AnimationStateMachineGraphInstance : IDisposable
    {
        public Playable RootPlayable { get; }


        public virtual void Dispose()
        {
        }
    }
}