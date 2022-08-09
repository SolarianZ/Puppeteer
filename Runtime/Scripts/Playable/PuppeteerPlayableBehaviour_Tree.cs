using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UDebug = UnityEngine.Debug;

namespace GBG.Puppeteer
{
    public partial class PuppeteerPlayableBehaviour
    {
        private readonly List<GraphLayer> _layers = new List<GraphLayer>();


        #region Layer

        public bool TryLayerNameToIndex(string layerName, out int index)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Name.Equals(layerName))
                {
                    index = i;
                    return true;
                }
            }

            index = default;
            return false;
        }

        public bool HasLayer(string layerName)
        {
            return TryLayerNameToIndex(layerName, out _);
        }

        public void PushLayer(string layerName, AvatarMask avatarMask = null,
            bool isAdditive = false, float weight = 0f) // ik?
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                throw new System.ArgumentNullException(nameof(layerName),
                    "Layer name can not be empty.");
            }

            if (HasLayer(layerName))
            {
                throw new System.InvalidOperationException($"Layer {layerName} already exist.");
            }

            var layer = new GraphLayer(layerName, isAdditive, avatarMask, weight);
            _layers.Add(layer);

            PushGraphLayer(layer);
        }

        public string PopLayer()
        {
            if (_layers.Count == 0)
            {
                return string.Empty;
            }

            var layerIndex = _layers.Count - 1;
            var layer = _layers[layerIndex];
            _layers.RemoveAt(layerIndex);

            PopGraphLayer();

            return layer.Name;
        }

        public void SetLayerWeight(string layerName, float weight)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                throw new System.ArgumentException($"Layer {layerName} not exist.", nameof(layerName));
            }

            if (weight < 0 || weight > 1)
            {
                UDebug.LogWarning($"{_logPerfix} Clamp weight({weight}) into [0f,1f].");
                weight = Mathf.Clamp01(weight);
            }

            var layer = _layers[layerIndex];
            layer.Weight = weight;

            _layerMixerPlayable.SetInputWeight(layerIndex, weight);
        }

        #endregion


        #region State

        public bool HasState(string layerName, string stateName)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                return false;
            }

            var layer = _layers[layerIndex];
            return layer.HasState(stateName);
        }

        public void AddState(string layerName, string stateName, AnimationClip animClip,
            float playbackSpeed = 1f)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                throw new System.ArgumentException(
                    $"Layer {layerName} not exist, please add layer first.", nameof(layerName));
            }

            var layer = _layers[layerIndex];
            if (layer.HasState(stateName))
            {
                throw new System.ArgumentException(
                    $"State {stateName} already exist in layer {layerName}.", nameof(stateName));
            }

            layer.AddState(stateName, animClip, playbackSpeed);

            // todo need rebuild graph
        }

        //public void AddBlendTree1DState(string layerName, string stateName, BlendTree1DInfo)
        //{
        //    // BlendTree1DInfo: AnimationClip - blendParam threshold
        //    throw new System.NotImplementedException();
        //}

        //public void AddBlendTree2DState(string layerName, string stateName, BlendTree2DInfo)
        //{
        //    throw new System.NotImplementedException();
        //}

        public bool RemoveState(string layerName, string stateName)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                return false;
            }

            var layer = _layers[layerIndex];
            if (!layer.RemoveState(stateName, out var state))
            {
                return false;
            }

            // todo need rebuild graph

            return true;
        }

        public void SetStatePlaybackSpeed(string layerName, string stateName, float speed)
        {
            var state = GetStateWithException(layerName, stateName);
            state.PlaybackSpeed = speed;

            // todo need rebuild graph
        }


        private void SetStateWeight(string layerName, string stateName, float weight)
        {
            var state = GetStateWithException(layerName, stateName);

            // todo set state weight
        }

        private GraphClipState GetStateWithException(string layerName, string stateName)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                throw new System.ArgumentException($"Layer {layerName} not exist.",
                    nameof(layerName));
            }

            var layer = _layers[layerIndex];
            if (!layer.TryGetState(stateName, out var state))
            {
                throw new System.ArgumentException($"State {stateName} not exist.",
                    nameof(stateName));
            }

            return state;
        }

        #endregion
    }
}