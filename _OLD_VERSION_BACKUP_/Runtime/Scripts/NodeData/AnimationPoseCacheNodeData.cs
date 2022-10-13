using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    // TODO: AnimationPoseCacheNode
    [Serializable]
    public class AnimationPoseCacheNodeData : AnimationNodeData
    {
        // TODO: A pose cache may be referenced by multi nodes,
        // but its PrepareFrame method should only be called once per frame.

        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodeTable,
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