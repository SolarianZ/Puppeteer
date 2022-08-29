using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Parameter;
using GBG.Puppeteer.NodeInstance;

namespace GBG.Puppeteer.Node
{
    [Serializable]
    public class AnimationClipNode : AnimationNode
    {
        [SerializeField]
        private AnimationClip _animationClip;

        [SerializeField]
        private ParamNameOrValue _useExplicitTime;

        [SerializeField]
        private ParamNameOrValue _explicitTime;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph, 
            Animator animator, Dictionary<string, AnimationNode> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            return new AnimationClipInstance(graph, _animationClip,
                _useExplicitTime.GetParamInfo(parameters, ParamType.Bool),
                _explicitTime.GetParamInfo(parameters, ParamType.Float),
                PlaybackSpeed.GetParamInfo(parameters, ParamType.Float));
        }
    }
}