using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public partial class PuppeteerPlayableBehaviour
    {
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


        #region State

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
            timeOffset = GetFixedTimeOffset(timeOffset, timeMode, state.Clip);
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
            timeOffset = GetFixedTimeOffset(timeOffset, timeMode, state.Clip);
            var toPlayable = state.CreatePlayable(_graph, timeOffset);

            // mixer
            var animMixerPlayable = AnimationMixerPlayable.Create(_graph);
            animMixerPlayable.AddInput(fromPlayable, 0, 1f);
            animMixerPlayable.AddInput(toPlayable, 0, 0f);
            layerRootMixer.ConnectInput(0, animMixerPlayable, 0);
            layerRootMixer.SetInputWeight(0, 1);

            // only allow one active cross fade per layer
            var crossFadeInfo = new CrossFadeInfo
            {
                LayerName = layerName,
                From = fromPlayable,
                To = toPlayable,
                Receiver = animMixerPlayable,
                Duration = GetFixedTimeOffset(fadeDuration, timeMode, state.Clip),
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


        private static float GetFixedTimeOffset(float timeOffset, TimeMode timeMode, AnimationClip clip)
        {
            if (timeMode == TimeMode.NormalizedTime && clip)
            {
                timeOffset %= 1;
                timeOffset *= clip.length;
            }

            return timeOffset;
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


        private static void SwapInputs(Playable playable, int fromIndex, int toIndex)
        {
            var inputA = playable.GetInput(fromIndex);
            var weightA = playable.GetInputWeight(fromIndex);
            var inputB = playable.GetInput(toIndex);
            var weightB = playable.GetInputWeight(toIndex);

            playable.DisconnectInput(toIndex);
            playable.DisconnectInput(fromIndex);

            playable.ConnectInput(toIndex, inputA, 0, weightA);
            playable.ConnectInput(fromIndex, inputB, 0, weightB);
        }


        struct CrossFadeInfo
        {
            public string LayerName;

            public Playable From;

            public Playable To;

            public AnimationMixerPlayable Receiver;

            public float Duration;

            public float Timer;


            public void Evaluate(float deltaTime)
            {
                if (IsDone())
                {
                    return;
                }

                if (!From.IsValid())
                {
                    Timer = Duration + 1;

                    if (!To.IsValid())
                    {
                        return;
                    }

                    Receiver.SetInputWeight(0, 0);
                    Receiver.SetInputWeight(1, 1);

                    return;
                }

                if (!To.IsValid())
                {
                    Timer = Duration + 1;

                    if (!From.IsValid())
                    {
                        return;
                    }

                    Receiver.SetInputWeight(0, 0);
                    Receiver.SetInputWeight(1, 0);

                    return;
                }

                Timer += deltaTime;
                var progress = Mathf.Lerp(0, 1, Timer / Duration);

                Receiver.SetInputWeight(0, 1 - progress);
                Receiver.SetInputWeight(1, progress);
            }

            public bool IsDone()
            {
                return Timer >= Duration;
            }
        }

        #endregion
    }
}