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

        public Playable CreatePlayable(PlayableGraph graph, float fixedTime)
        {
            var animClipPlayable = AnimationClipPlayable.Create(graph, Clip);
            animClipPlayable.SetTime(fixedTime);
            animClipPlayable.SetSpeed(PlaybackSpeed);

            return animClipPlayable;
        }
    }
}
