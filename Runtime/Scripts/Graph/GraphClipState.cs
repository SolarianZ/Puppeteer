using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    internal class GraphClipState
    {
        public string Name { get; }

        public AnimationClip Clip { get; }

        public float PlaybackSpeed { get; set; }


        public GraphClipState(string name, AnimationClip clip, float playbackSpeed = 1.0f)
        {
            Name = name;
            Clip = clip;
            PlaybackSpeed = playbackSpeed;
        }

        public AnimationMixerPlayable CreatePlayable(PlayableGraph graph, float fixedTime)
        {
            var animClipPlayable = AnimationClipPlayable.Create(graph, Clip);
            animClipPlayable.SetTime(fixedTime);
            animClipPlayable.SetSpeed(PlaybackSpeed);
            var animMixerPlayable = AnimationMixerPlayable.Create(graph);
            animMixerPlayable.AddInput(animClipPlayable, 0, 1);

            return animMixerPlayable;
        }
    }
}
