using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public partial class Puppeteer
    {
        public int LayerCount => _layers.Count;

        private readonly List<GraphLayer> _layers = new List<GraphLayer>();


        #region Layer

        public bool TryLayerNameToIndex(string layerName, out int index)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].LayerName.Equals(layerName))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public bool HasLayer(string layerName)
        {
            return TryLayerNameToIndex(layerName, out _);
        }

        public int PushLayer(string layerName, AvatarMask avatarMask = null,
            bool isAdditive = false, float weight = 0f)
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

            return _layers.Count - 1;
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

            return layer.LayerName;
        }

        public void SetLayerWeight(string layerName, float weight)
        {
            if (!TryLayerNameToIndex(layerName, out var layerIndex))
            {
                throw new System.ArgumentException($"Layer {layerName} not exist.",
                    nameof(layerName));
            }

            if (weight < 0 || weight > 1)
            {
                throw new System.ArgumentException("Layer weight should be in range of [0f,1f].",
                    nameof(weight));
            }

            var layer = _layers[layerIndex];
            layer.Weight = weight;

            _layerMixerPlayable.SetInputWeight(layerIndex, weight);
        }

        /** Why not a "PrepareLayer" method?
         * Users should know the order of all layers,
         * "PrepareLayer" are apt to make users ignore layer order.
         */

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

            state.Destroy();

            return true;
        }

        public void SetStatePlaybackSpeed(string layerName, string stateName, float speed)
        {
            var state = GetStateWithException(layerName, stateName);
            state.PlaybackSpeed = speed;
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
                throw new System.ArgumentException($"State {stateName} not exist in layer {layerName}.",
                    nameof(stateName));
            }

            return state;
        }

        #endregion
    }
}
