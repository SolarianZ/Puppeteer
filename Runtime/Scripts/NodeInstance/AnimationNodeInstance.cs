using System;
using GBG.Puppeteer.Parameter;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public abstract class AnimationNodeInstance : IDisposable
    {
        public abstract Playable Playable { get; }

        public bool IsPlaying { get; private set; } = true;


        private readonly ParamInfo _playbackSpeed;

        private bool _isPlaybackSpeedDirty;


        protected AnimationNodeInstance(ParamInfo playbackSpeed)
        {
            _playbackSpeed = playbackSpeed;
            _playbackSpeed.OnValueChanged += OnPlaybackSpeedChanged;
            OnPlaybackSpeedChanged(_playbackSpeed);
        }


        public void Play()
        {
            IsPlaying = true;
            Playable.SetSpeed<Playable>(_playbackSpeed.GetFloat());
        }

        public void Pause()
        {
            IsPlaying = false;
            Playable.SetSpeed<Playable>(0);
        }


        private void OnPlaybackSpeedChanged(ParamInfo _)
        {
            _isPlaybackSpeedDirty = true;
        }


        public virtual void PrepareFrame(float deltaTime)
        {
            if (_isPlaybackSpeedDirty)
            {
                _isPlaybackSpeedDirty = false;
                if (IsPlaying)
                {
                    Playable.SetSpeed<Playable>(_playbackSpeed.GetFloat());
                }
            }
        }

        // public virtual void ProcessFrame(float deltaTime)
        // {
        // }

        public virtual void Dispose()
        {
            _playbackSpeed.OnValueChanged -= OnPlaybackSpeedChanged;
        }
    }
}
