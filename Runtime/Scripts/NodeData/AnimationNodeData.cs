using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public abstract class AnimationNodeData
    {
        public string Guid => _guid;

        [SerializeField]
        private string _guid;


        protected ParamNameOrValue PlaybackSpeed => _playbackSpeed;

        [SerializeField]
        private ParamNameOrValue _playbackSpeed;


        public abstract AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters);


        public AnimationNodeData Clone(string newGuid)
        {
            var clone = InternalDeepClone();
            clone._guid = newGuid;
            return clone;
        }

        protected abstract AnimationNodeData InternalDeepClone();
    }
}