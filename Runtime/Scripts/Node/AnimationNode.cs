using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.Node
{
    [Serializable]
    public abstract class AnimationNode
    {
        public string Guid => _guid;

        [SerializeField]
        private string _guid;


        protected ParamNameOrValue PlaybackSpeed => _playbackSpeed;

        [SerializeField]
        private ParamNameOrValue _playbackSpeed;

       
        public abstract AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNode> nodes,
            Dictionary<string, ParamInfo> parameters);
    }
}