using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.Puppeteer
{
    internal class GraphLayer : IEnumerable<GraphClipState>
    {
        public string LayerName { get; }

        public bool IsAdditive { get; set; }

        public AvatarMask AvatarMask { get; }

        public float Weight { get; set; }

        public int StateCount => _states.Count;


        private readonly Dictionary<string, GraphClipState> _states
            = new Dictionary<string, GraphClipState>();


        public GraphLayer(string layerName, bool isAdditive = false,
            AvatarMask avatarMask = null, float weight = 0f)
        {
            LayerName = layerName;
            IsAdditive = isAdditive;
            AvatarMask = avatarMask;
            Weight = weight;
        }

        public bool HasState(string stateName)
        {
            return _states.ContainsKey(stateName);
        }

        public void AddState(string stateName, AnimationClip clip,
            float playbackSpeed = 1.0f)
        {
            if (HasState(stateName))
            {
                throw new System.ArgumentException($"State {stateName} already exist.",
                    nameof(stateName));
            }

            var state = new GraphClipState(stateName, clip, playbackSpeed);
            _states.Add(stateName, state);
        }

        public bool RemoveState(string stateName, out GraphClipState state)
        {
            return _states.Remove(stateName, out state);
        }

        public bool TryGetState(string stateName, out GraphClipState state)
        {
            return _states.TryGetValue(stateName, out state);
        }

        public IEnumerator<GraphClipState> GetEnumerator()
        {
            return _states.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
