using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Parameter;
using GBG.Puppeteer.NodeInstance;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationClipNodeData : AnimationNodeData
    {
        internal AnimationClip AnimationClip => _animationClip;

        [SerializeField]
        private AnimationClip _animationClip;


        internal ParamNameOrValue UseExplicitTime => _useExplicitTime;

        [SerializeField]
        private ParamNameOrValue _useExplicitTime;


        internal ParamNameOrValue ExplicitTime => _explicitTime;

        [SerializeField]
        private ParamNameOrValue _explicitTime;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            return new AnimationClipInstance(graph, _animationClip,
                _useExplicitTime.GetParamInfo(parameters, ParamType.Bool),
                _explicitTime.GetParamInfo(parameters, ParamType.Float),
                PlaybackSpeed.GetParamInfo(parameters, ParamType.Float));
        }


        protected override AnimationNodeData InternalDeepClone()
        {
            return new AnimationClipNodeData()
            {
                _animationClip = this._animationClip,
                _useExplicitTime = (ParamNameOrValue)this._useExplicitTime.Clone(),
                _explicitTime = (ParamNameOrValue)this._explicitTime.Clone()
            };
        }
    }
}
