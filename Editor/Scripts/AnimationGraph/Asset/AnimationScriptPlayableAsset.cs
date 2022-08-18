using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    // todo move to Runtime
    [CreateAssetMenu(fileName = nameof(AnimationScriptPlayableAsset),
        menuName = "Puppeteer/Animation Script Playable Asset")]
    public abstract class AnimationScriptPlayableAsset : PlayableAsset
    {
        public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return CreateAnimationScriptPlayable(graph, owner);
        }


        protected abstract AnimationScriptPlayable CreateAnimationScriptPlayable(PlayableGraph graph, GameObject owner);
    }
}
