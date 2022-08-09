using System.Collections.Generic;
using UnityEngine;

namespace GBG.Puppeteer
{
    internal class GraphLayer
    {
        public string Name { get; }

        public bool IsAdditive { get; }

        public AvatarMask Mask { get; set; }

        public float Weight { get; set; }


        private readonly Dictionary<string, GraphClipState> _states
            = new Dictionary<string, GraphClipState>();


        public GraphLayer(string name, bool isAdditive = false,
            AvatarMask mask = null, float weight = 0f)
        {
            Name = name;
            IsAdditive = isAdditive;
            Mask = mask;
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
    }
}
