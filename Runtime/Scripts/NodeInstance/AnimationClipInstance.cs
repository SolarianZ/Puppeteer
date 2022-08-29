using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationClipInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly ParamInfo _useExplicitTime;

        private readonly ParamInfo _explicitTime;

        private readonly ParamInfo _playbackSpeed;


        public AnimationClipInstance(PlayableGraph graph, AnimationClip animClip, ParamInfo useExplicitTime,
            ParamInfo explicitTime, ParamInfo playbackSpeed)
        {
            Playable = AnimationClipPlayable.Create(graph, animClip);

            _useExplicitTime = useExplicitTime;
            _useExplicitTime.OnValueChanged += OnUseExplicitTimeChanged;
            OnUseExplicitTimeChanged(_useExplicitTime);

            _explicitTime = explicitTime;
            _explicitTime.OnValueChanged += OnExplicitTimeChanged;
            OnExplicitTimeChanged(_explicitTime);

            _playbackSpeed = playbackSpeed;
            _playbackSpeed.OnValueChanged += OnPlaybackSpeedChanged;
            OnPlaybackSpeedChanged(_playbackSpeed);
        }


        private void OnUseExplicitTimeChanged(ParamInfo param)
        {
            var useExplicitTime = param.GetBool();
            if (useExplicitTime)
            {
                Playable.Pause();
                Playable.SetTime(_explicitTime.GetFloat());
            }
            else
            {
                Playable.Play();
            }
        }

        private void OnExplicitTimeChanged(ParamInfo param)
        {
            if (Playable.GetPlayState() == PlayState.Paused)
            {
                Playable.SetTime(param.GetFloat());
            }
        }

        private void OnPlaybackSpeedChanged(ParamInfo param)
        {
            Playable.SetSpeed(param.GetFloat());
        }


        public override void Dispose()
        {
            _useExplicitTime.OnValueChanged -= OnUseExplicitTimeChanged;
            _explicitTime.OnValueChanged -= OnExplicitTimeChanged;
            _playbackSpeed.OnValueChanged -= OnPlaybackSpeedChanged;
            
            base.Dispose();
        }
    }
}