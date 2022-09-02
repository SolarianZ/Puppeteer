using System;
using GBG.Puppeteer.Parameter;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public abstract class AnimationNodeInstance : IDisposable
    {
        public abstract Playable Playable { get; }

        
        private readonly ParamInfo _playbackSpeed;

        private bool _isPlaybackSpeedDirty;


        protected AnimationNodeInstance(ParamInfo playbackSpeed)
        {
            _playbackSpeed = playbackSpeed;
            _playbackSpeed.OnValueChanged += OnPlaybackSpeedChanged;
            OnPlaybackSpeedChanged(_playbackSpeed);
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
                Playable.SetSpeed(_playbackSpeed.GetFloat());
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
