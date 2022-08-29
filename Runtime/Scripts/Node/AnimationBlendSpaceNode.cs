using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Node
{
    public enum BlendSpaceDimension
    {
        OneD,

        TwoD,
    }

    // TODO: AnimationBlendSpaceNode
    [Serializable]
    public class AnimationBlendSpaceNode : AnimationNode
    {
        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph, 
            Animator animator, Dictionary<string, AnimationNode> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            throw new NotImplementedException();
        }
    }
}