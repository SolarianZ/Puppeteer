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

        private bool _isUseExplicitTimeDirty;

        private bool _isExplicitTimeDirty;


        public AnimationClipInstance(PlayableGraph graph, AnimationClip animClip, ParamInfo useExplicitTime,
            ParamInfo explicitTime, ParamInfo playbackSpeed) : base(playbackSpeed)
        {
            Playable = AnimationClipPlayable.Create(graph, animClip);

            _useExplicitTime = useExplicitTime;
            _useExplicitTime.OnValueChanged += OnUseExplicitTimeChanged;
            OnUseExplicitTimeChanged(_useExplicitTime);

            _explicitTime = explicitTime;
            _explicitTime.OnValueChanged += OnExplicitTimeChanged;
            OnExplicitTimeChanged(_explicitTime);
        }


        private void OnUseExplicitTimeChanged(ParamInfo param)
        {
            _isUseExplicitTimeDirty = true;
        }

        private void OnExplicitTimeChanged(ParamInfo param)
        {
            _isExplicitTimeDirty = true;
        }


        public override void PrepareFrame(float deltaTime)
        {
            base.PrepareFrame(deltaTime);

            if (_isUseExplicitTimeDirty)
            {
                _isUseExplicitTimeDirty = false;
                var useExplicitTime = _useExplicitTime.GetBool();
                if (useExplicitTime)
                {
                    Pause();
                    Playable.SetTime(_explicitTime.GetFloat());
                }
                else
                {
                    Play();
                }
            }

            if (_isExplicitTimeDirty)
            {
                _isExplicitTimeDirty = false;
                Playable.SetTime(_explicitTime.GetFloat());

                if (_useExplicitTime.GetBool())
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }
        }

        public override void Dispose()
        {
            _useExplicitTime.OnValueChanged -= OnUseExplicitTimeChanged;
            _explicitTime.OnValueChanged -= OnExplicitTimeChanged;

            base.Dispose();
        }
    }
}
