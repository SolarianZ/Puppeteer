using System;
using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [CreateAssetMenu(fileName = nameof(AnimationLayerMixerGraphAsset),
        menuName = "Puppeteer/Animation Layer Mixer Graph")]
    public class AnimationLayerMixerGraphAsset : AnimationGraphAsset
    {
        public override Type RootNodeType => typeof(AnimationLayerMixerNode);
    }
}