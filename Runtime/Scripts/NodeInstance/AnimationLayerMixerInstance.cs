using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationLayerMixerInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly AnimationNodeInstance[] _inputs;

        private readonly ParamInfo[] _inputWeights;

        private bool _isInputWeightDirty = true;


        public AnimationLayerMixerInstance(PlayableGraph graph, AnimationNodeInstance[] inputs,
            ParamInfo[] inputWeights, ParamInfo playbackSpeed,
            bool[] layerAdditiveStates, AvatarMask[] layerAvatarMasks) : base(playbackSpeed)
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
        }


        private void OnInputWeightChanged(ParamInfo param)
        {
            _isInputWeightDirty = true;
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

            _isInputWeightDirty = false;

            // Input weights
            for (int i = 0; i < _inputWeights.Length; i++)
            {
                var originalWeight = _inputWeights[i].GetFloat();
                Playable.SetInputWeight(i, originalWeight);
            }
        }

        public override void Dispose()
        {
            foreach (var inputWeight in _inputWeights)
            {
                inputWeight.OnValueChanged -= OnInputWeightChanged;
            }

            base.Dispose();
        }
    }
}
