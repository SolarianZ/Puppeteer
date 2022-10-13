using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationClipNodeData : AnimationNodeData
    {
        public AnimationClip AnimationClip
        {
            get => _animationClip;
            set => _animationClip = value;
        }

        [SerializeField]
        private AnimationClip _animationClip;


        public ParamNameOrValue UseExplicitTime
        {
            get => _useExplicitTime;
            set => _useExplicitTime = value;
        }

        [SerializeField]
        private ParamNameOrValue _useExplicitTime;


        public ParamNameOrValue ExplicitTime
        {
            get => _explicitTime;
            set => _explicitTime = value;
        }

        [SerializeField]
        private ParamNameOrValue _explicitTime;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            return new AnimationClipInstance(graph, _animationClip,
                _useExplicitTime.GetParamInfo(paramTable, ParamType.Bool),
                _explicitTime.GetParamInfo(paramTable, ParamType.Float),
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationClipNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var animClipNodeData = (AnimationClipNodeData)clone;
            animClipNodeData._animationClip = _animationClip;
            animClipNodeData._useExplicitTime = (ParamNameOrValue)_useExplicitTime.Clone();
            animClipNodeData._explicitTime = (ParamNameOrValue)_explicitTime.Clone();
        }

        #endregion
    }
}
