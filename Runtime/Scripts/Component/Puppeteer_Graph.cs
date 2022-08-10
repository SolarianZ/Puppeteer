using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public partial class Puppeteer
    {
        #region State

        public StateInfo GetStateInfo(string layerName, string stateName)
        {
            var state = GetStateWithException(layerName, stateName);
            var stateInfo = new StateInfo(layerName, state);

            return stateInfo;
        }

        public void GetActiveStateInfo(string layerName, IList<StateInfo> result)
        {
            result.Clear();

            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                throw new System.ArgumentException($"Layer {layerName} not exist.",
                    nameof(layerName));
            }

            var layer = _layers[layerIndex];
            foreach (var state in layer)
            {
                if (state.IsPlaying)
                {
                    result.Add(new StateInfo(layerName, state));
                }
            }
        }

        public void PlayState(string layerName, string stateName, float timeOffset = 0f,
            TimeMode timeMode = TimeMode.FixedTime)
        {
            var layerRootMixer = GetLayerRootMixer(layerName);

            // remove old state
            var oldPlayable = layerRootMixer.GetInput(0);
            if (oldPlayable.IsValid())
            {
                oldPlayable.Destroy();
                RemoveCrossFade(layerName);
            }

            // create new state
            var state = GetStateWithException(layerName, stateName);
            timeOffset = TimeTool.GetFixedTime(timeOffset, timeMode, state.Clip);
            var statePlayable = state.CreatePlayable(_graph, timeOffset);

            // connect
            layerRootMixer.ConnectInput(0, statePlayable, 0);
            layerRootMixer.SetInputWeight(0, 1);
        }

        public void CrossFadeState(string layerName, string destStateName, float fadeDuration,
            float timeOffset = 0f, TimeMode timeMode = TimeMode.FixedTime)
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
            if (!fromPlayable.IsValid())
            {
                PlayState(layerName, destStateName, timeOffset, timeMode);
                return;
            }

            // to state
            var toState = GetStateWithException(layerName, destStateName);
            timeOffset = TimeTool.GetFixedTime(timeOffset, timeMode, toState.Clip);
            var toPlayable = toState.CreatePlayable(_graph, timeOffset);

            // cross fade mixer
            var crossFadeMixer = AnimationMixerPlayable.Create(_graph);
            crossFadeMixer.AddInput(fromPlayable, 0, 1f);
            crossFadeMixer.AddInput(toPlayable, 0, 0f);
            layerRootMixer.ConnectInput(0, crossFadeMixer, 0, 1);

            // only allow one active cross fade per layer
            var crossFadeDuration = TimeTool.GetFixedTime(fadeDuration, timeMode, toState.Clip);
            var crossFadeInfo = new CrossFadeInfo(layerName, fromPlayable,
                toPlayable, crossFadeMixer, crossFadeDuration);
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


        private AnimationMixerPlayable GetLayerRootMixer(string layerName)
        {
            TryLayerNameToIndex(layerName, out var layerIndex);
            var rootMixer = _layerMixerPlayable.GetInput(layerIndex);
            return (AnimationMixerPlayable)rootMixer;
        }

        #endregion


        #region Cross Fade

        private readonly List<CrossFadeInfo> _activeCrossFades = new List<CrossFadeInfo>();


        private bool RemoveCrossFade(string layerName)
        {
            for (int i = 0; i < _activeCrossFades.Count; i++)
            {
                if (_activeCrossFades[i].LayerName.Equals(layerName))
                {
                    _activeCrossFades.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void ProcessCrossFades(float deltaTime)
        {
            for (int i = 0; i < _activeCrossFades.Count; i++)
            {
                var crossFadeInfo = _activeCrossFades[i];
                crossFadeInfo.Evaluate(deltaTime);
                if (crossFadeInfo.IsDone())
                {
                    _activeCrossFades.RemoveAt(i--);

                    // remove temp cross fade mixer
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
            private readonly bool _isValid;

            public readonly string LayerName;

            public readonly Playable From;

            public readonly Playable To;

            public readonly AnimationMixerPlayable Mixer;

            private readonly float _duration;

            private float _timer;


            public CrossFadeInfo(string layerName, Playable from, Playable to,
                AnimationMixerPlayable mixer, float duration)
            {
                LayerName = layerName;
                From = from;
                To = to;
                Mixer = mixer;
                _duration = duration;
                _timer = 0;
                _isValid = true;
            }

            public void Evaluate(float deltaTime)
            {
                Assert.IsTrue(IsValid());
                Assert.IsTrue(From.IsValid());
                Assert.IsTrue(To.IsValid());
                Assert.IsTrue(Mixer.IsValid());

                if (IsDone())
                {
                    return;
                }

                _timer += deltaTime;
                var progress = Mathf.Lerp(0, 1, _timer / _duration);

                Mixer.SetInputWeight(0, 1 - progress);
                Mixer.SetInputWeight(1, progress);
            }

            public bool IsDone()
            {
                return _timer >= _duration;
            }

            public bool IsValid()
            {
                return _isValid;
            }
        }

        #endregion


        #region Layer

        private void PushGraphLayer(GraphLayer layer)
        {
            var layerRootMixer = AnimationMixerPlayable.Create(_graph, 1);

            // when disconnect input, the input port will not be removed,
            // so we need to check if there are any idle ports
            uint? layerIndex = null;
            for (int i = 0; i < _layerMixerPlayable.GetInputCount(); i++)
            {
                if (_layerMixerPlayable.GetInput(i).IsValid())
                {
                    continue;
                }

                layerIndex = (uint?)i;
            }
            layerIndex ??= (uint)_layerMixerPlayable.AddInput(layerRootMixer, 0, layer.Weight);

            _layerMixerPlayable.SetLayerAdditive(layerIndex.Value, layer.IsAdditive);
            if (layer.AvatarMask)
            {
                _layerMixerPlayable.SetLayerMaskFromAvatarMask(layerIndex.Value, layer.AvatarMask);
            }
        }

        private void PopGraphLayer()
        {
            // layer has been removed from stack, so there is no need to minus one
            var layerIndex = _layers.Count;//-1;
            var layerInputPlayable = _layerMixerPlayable.GetInput(layerIndex);
            layerInputPlayable.Destroy();

            _layerMixerPlayable.SetLayerAdditive((uint)layerIndex, false);
            //_layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)layerIndex, null); // no way
            _layerMixerPlayable.SetInputWeight(layerIndex, 0);
            _layerMixerPlayable.DisconnectInput(layerIndex); // will not remove input port
        }

        #endregion
    }
}
