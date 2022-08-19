using System;
using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [CreateAssetMenu(fileName = nameof(AnimationMixerGraphAsset),
        menuName = "Puppeteer/Animation Mixer Graph")]
    public class AnimationMixerGraphAsset : AnimationGraphAsset
    {
        public override Type RootNodeType => typeof(AnimationMixerNode);
    }
}