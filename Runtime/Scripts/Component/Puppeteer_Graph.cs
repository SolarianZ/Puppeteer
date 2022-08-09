using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public partial class Puppeteer
    {
        #region State

        public StateInfo GetStateInfo()
        {
            throw new System.NotImplementedException();
        }

        public void PlayState(string layerName, string stateName, float timeOffset = 0f,
            TimeMode timeMode = TimeMode.NormalizedTime)
        {
            var layerRootMixer = GetLayerRootMixer(layerName);

            // remove old state
            var oldPlayable = layerRootMixer.GetInput(0);
            if (oldPlayable.IsValid())
            {
                oldPlayable.Destroy();
            }

            // create new state
            var state = GetStateWithException(layerName, stateName);
            timeOffset = GetFixedTime(timeOffset, timeMode, state.Clip);
            var animMixerPlayable = state.CreatePlayable(_graph, timeOffset);

            // connect
            layerRootMixer.ConnectInput(0, animMixerPlayable, 0);
            layerRootMixer.SetInputWeight(0, 1);
        }

        public void CrossFadeState(string layerName, string destStateName, float fadeDuration,
            float timeOffset = 0f, TimeMode timeMode = TimeMode.NormalizedTime)
        {
            if (fadeDuration <= Mathf.Epsilon)
            {
                PlayState(layerName, destStateName, timeOffset, timeMode);
                return;
            }

            var layerRootMixer = GetLayerRootMixer(layerName);

            // from state
            var fromPlayable = layerRootMixer.GetInput(0);
            layerRootMixer.DisconnectInput(0);

            // to state
            var state = GetStateWithException(layerName, destStateName);
            timeOffset = GetFixedTime(timeOffset, timeMode, state.Clip);
            var toPlayable = state.CreatePlayable(_graph, timeOffset);

            // mixer
            var crossFadeMixer = AnimationMixerPlayable.Create(_graph);
            crossFadeMixer.AddInput(fromPlayable, 0, 1f);
            crossFadeMixer.AddInput(toPlayable, 0, 0f);
            layerRootMixer.ConnectInput(0, crossFadeMixer, 0, 1);

            // only allow one active cross fade per layer
            var crossFadeInfo = new CrossFadeInfo
            {
                LayerName = layerName,
                From = fromPlayable,
                To = toPlayable,
                Mixer = crossFadeMixer,
                Duration = GetFixedTime(fadeDuration, timeMode, state.Clip),
                Timer = 0
            };
            var inserted = false;
            for (int i = 0; i < _activeCrossFades.Count; i++)
            {
                if (_activeCrossFades[i].LayerName.Equals(layerName))
                {
                    _activeCrossFades[i] = crossFadeInfo;
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                _activeCrossFades.Add(crossFadeInfo);
            }
        }


        private static float GetFixedTime(float time, TimeMode timeMode, AnimationClip clip)
        {
            if (timeMode == TimeMode.NormalizedTime && clip)
            {
                if (Mathf.Abs(time - 1) > Mathf.Epsilon)
                {
                    time %= 1;
                }
                time *= clip.length;
            }

            return time;
        }

        private AnimationMixerPlayable GetLayerRootMixer(string layerName)
        {
            TryLayerNameToIndex(layerName, out var layerIndex);
            //if (_layerMixerPlayable.GetInputCount() == 0)
            //{
            //    return AnimationMixerPlayable.Null;
            //}
            var rootMixer = _layerMixerPlayable.GetInput(layerIndex);
            return (AnimationMixerPlayable)rootMixer;
        }

        #endregion


        #region Cross Fade

        private readonly List<CrossFadeInfo> _activeCrossFades = new List<CrossFadeInfo>();


        private void ProcessCrossFades(float deltaTime)
        {
            for (int i = 0; i < _activeCrossFades.Count; i++)
            {
                var crossFadeInfo = _activeCrossFades[i];
                crossFadeInfo.Evaluate(deltaTime);
                if (crossFadeInfo.IsDone())
                {
                    _activeCrossFades.RemoveAt(i--);

                    // remove temp mixer of cross fade
                    var layerRootMixer = GetLayerRootMixer(crossFadeInfo.LayerName);
                    layerRootMixer.DisconnectInput(0);
                    crossFadeInfo.Mixer.DisconnectInput(1);
                    layerRootMixer.ConnectInput(0, crossFadeInfo.To, 0, 1);
                    crossFadeInfo.Mixer.Destroy();
                }
                else
                {
                    _activeCrossFades[i] = crossFadeInfo;
                }
            }
        }


        internal struct CrossFadeInfo
        {
            public string LayerName;

            public Playable From;

            public Playable To;

            public AnimationMixerPlayable Mixer;

            public float Duration;

            public float Timer;


            public void Evaluate(float deltaTime)
            {
                if (IsDone())
                {
                    return;
                }

                Timer += deltaTime;
                var progress = Mathf.Lerp(0, 1, Timer / Duration);

                Mixer.SetInputWeight(0, 1 - progress);
                Mixer.SetInputWeight(1, progress);
            }

            public bool IsDone()
            {
                return Timer >= Duration;
            }
        }

        #endregion


        #region Layer

        private void PushGraphLayer(GraphLayer layer)
        {
            var animMixerPlayable = AnimationMixerPlayable.Create(_graph, 1);

            uint? layerIndex = null;
            for (int i = 0; i < _layerMixerPlayable.GetInputCount(); i++)
            {
                if (_layerMixerPlayable.GetInput(i).IsValid())
                {
                    continue;
                }

                layerIndex = (uint?)i;
            }
            layerIndex ??= (uint)_layerMixerPlayable.AddInput(animMixerPlayable, 0, layer.Weight);

            _layerMixerPlayable.SetLayerAdditive(layerIndex.Value, layer.IsAdditive);
            if (layer.Mask)
            {
                _layerMixerPlayable.SetLayerMaskFromAvatarMask(layerIndex.Value, layer.Mask);
            }
        }

        private void PopGraphLayer()
        {
            var layerIndex = _layerMixerPlayable.GetInputCount() - 1;
            var layerInputPlayable = _layerMixerPlayable.GetInput(layerIndex);
            layerInputPlayable.Destroy();

            _layerMixerPlayable.SetLayerAdditive((uint)layerIndex, false);
            _layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)layerIndex, null);
            _layerMixerPlayable.SetInputWeight(layerIndex, 0);
            _layerMixerPlayable.DisconnectInput(layerIndex); // will not remove input port
        }

        #endregion
    }
}
