using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public class PuppeteerPlayableBehaviour : PlayableBehaviour
    {
        // https://docs.unity3d.com/ScriptReference/Playables.Playable.html
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            // todo build playable graph hierarchy
        }

        // may not use this method
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
        }
    }
}
