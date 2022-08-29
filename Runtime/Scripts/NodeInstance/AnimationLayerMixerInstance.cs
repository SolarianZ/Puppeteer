using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationLayerMixerInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly AnimationNodeInstance[] _inputs;

        private readonly ParamInfo[] _inputWeights;

        private readonly ParamInfo _playbackSpeed;

        private bool _isInputWeightDirty = true;


        public AnimationLayerMixerInstance(PlayableGraph graph, AnimationNodeInstance[] inputs,
            ParamInfo[] inputWeights, ParamInfo playbackSpeed,
            bool[] layerAdditiveStates, AvatarMask[] layerAvatarMasks)
        {
            var layerMixer = AnimationLayerMixerPlayable.Create(graph);
            Playable = layerMixer;

            _inputs = inputs;
            for (var i = 0; i < _inputs.Length; i++)
            {
                var input = _inputs[i];
                layerMixer.AddInput(input.Playable, 0);
                layerMixer.SetLayerAdditive((uint)i, layerAdditiveStates[i]);
                if (layerAvatarMasks[i])
                {
                    layerMixer.SetLayerMaskFromAvatarMask((uint)i, layerAvatarMasks[i]);
                }
            }

            _inputWeights = inputWeights;
            foreach (var inputWeight in _inputWeights)
            {
                inputWeight.OnValueChanged += OnInputWeightChanged;
            }

            _playbackSpeed = playbackSpeed;
            _playbackSpeed.OnValueChanged += OnPlaybackSpeedChanged;
            OnPlaybackSpeedChanged(_playbackSpeed);
        }


        public override void PrepareFrame(float deltaTime)
        {
            base.PrepareFrame(deltaTime);

            foreach (var input in _inputs)
            {
                input.PrepareFrame(deltaTime);
            }
            
            if (!_isInputWeightDirty)
            {
                return;
            }

            // Total weight
            var totalWeight = 0f;
            for (int i = 0; i < _inputWeights.Length; i++)
            {
                totalWeight += _inputWeights[i].GetFloat();
            }

            // Relative weight
            for (int i = 0; i < _inputWeights.Length; i++)
            {
                var originalWeight = _inputWeights[i].GetFloat();
                Assert.IsTrue(originalWeight >= 0 && originalWeight <= 1);
                Assert.IsTrue(totalWeight >= originalWeight);

                var relativeWeight = Mathf.Approximately(0, totalWeight) ? 0 : originalWeight / totalWeight;
                Playable.SetInputWeight(i, relativeWeight);
            }

            _isInputWeightDirty = false;
        }


        private void OnInputWeightChanged(ParamInfo param)
        {
            _isInputWeightDirty = true;
        }

        private void OnPlaybackSpeedChanged(ParamInfo param)
        {
            Playable.SetSpeed(param.GetFloat());
        }


        public override void Dispose()
        {
            foreach (var inputWeight in _inputWeights)
            {
                inputWeight.OnValueChanged -= OnInputWeightChanged;
            }

            _playbackSpeed.OnValueChanged -= OnPlaybackSpeedChanged;
            
            base.Dispose();
        }
    }
}