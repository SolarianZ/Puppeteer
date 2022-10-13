using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationMixerInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly AnimationNodeInstance[] _inputs;

        private readonly ParamInfo[] _inputWeights;

        private bool _isInputWeightDirty = true;


        public AnimationMixerInstance(PlayableGraph graph, AnimationNodeInstance[] inputs,
            ParamInfo[] inputWeights, ParamInfo playbackSpeed) : base(playbackSpeed)
        {
            Playable = AnimationMixerPlayable.Create(graph);

            _inputs = inputs;
            foreach (var input in _inputs)
            {
                Playable.AddInput(input.Playable, 0);
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
