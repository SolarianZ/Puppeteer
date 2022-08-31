using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    public enum BlendSpaceDimension
    {
        OneD,

        TwoD,
    }

    // TODO: AnimationBlendSpaceNode
    [Serializable]
    public class AnimationBlendSpaceNodeData : AnimationNodeData
    {
        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            throw new NotImplementedException();
        }

        protected override AnimationNodeData CreateCloneInstance()
        {
            throw new NotImplementedException();
        }
    }
}