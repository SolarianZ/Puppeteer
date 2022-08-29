using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.Node
{
    [Serializable]
    public class AnimationSubGraphNode : AnimationNode
    {
        [SerializeField]
        private RuntimeAnimationGraph _subGraph;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNode> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            return new AnimationSubGraphInstance(graph, animator, _subGraph);
        }

        // TODO: ParamBinding
        public class ParamBinding
        {
        }
    }
}