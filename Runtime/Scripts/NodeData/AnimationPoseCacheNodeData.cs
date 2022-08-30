using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    // TODO: AnimationPoseCacheNode
    public class AnimationPoseCacheNodeData : AnimationNodeData
    {
        // TODO: A pose cache may be referenced by multi nodes,
        // but its PrepareFrame method should only be called once per frame.

        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            throw new System.NotImplementedException();
        }


        protected override AnimationNodeData InternalDeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}