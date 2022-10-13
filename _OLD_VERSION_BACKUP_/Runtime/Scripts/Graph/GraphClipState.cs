using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    internal class GraphClipState
    {
        public string StateName { get; }

        public AnimationClip Clip { get; }

        public float PlaybackSpeed
        {
            get { return Playable.IsValid() ? (float)Playable.GetSpeed() : _playbackSpeed; }
            set
            {
                _playbackSpeed = value;
                if (Playable.IsValid())
                {
                    Playable.SetSpeed<Playable>(value);
                }
            }
        }

        private float _playbackSpeed;

        public Playable Playable { get; private set; }

        public bool IsPlaying
        {
            get => Playable.IsValid() && Playable.GetPlayState() == PlayState.Playing;
        }

        public float Time
        {
            get { return Playable.IsValid() ? (float)Playable.GetTime() : 0; }
        }


        public GraphClipState(string stateName, AnimationClip clip, float playbackSpeed = 1.0f)
        {
            StateName = stateName;
            Clip = clip;
            _playbackSpeed = playbackSpeed;
        }

        public Playable CreatePlayable(PlayableGraph graph, float fixedTimeOffset)
        {
            var animClipPlayable = AnimationClipPlayable.Create(graph, Clip);
            animClipPlayable.SetTime(fixedTimeOffset);
            animClipPlayable.SetSpeed(PlaybackSpeed);
            //animClipPlayable.SetApplyFootIK(false);
            //animClipPlayable.SetApplyPlayableIK(false);

            Playable = animClipPlayable;

            return animClipPlayable;
        }

        public void Destroy()
        {
            if (Playable.IsValid())
            {
                Playable.Destroy();
            }
        }
    }
}
