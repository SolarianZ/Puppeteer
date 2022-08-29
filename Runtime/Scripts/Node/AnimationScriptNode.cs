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
    public class AnimationScriptNode : AnimationNode
    {
        public IReadOnlyList<InputInfo> InputInfos => _inputInfos;

        [SerializeField]
        private InputInfo[] _inputInfos;

        [SerializeField]
        private AnimationScriptableObject _animationScriptable;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNode> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            var inputInstances = new AnimationNodeInstance[InputInfos.Count];
            var inputWeights = new ParamInfo[InputInfos.Count];
            for (var i = 0; i < InputInfos.Count; i++)
            {
                var inputInfo = InputInfos[i];

                // Inputs
                var inputNode = nodes[inputInfo.InputNodeGuid];
                inputInstances[i] = inputNode.CreateNodeInstance(graph, animator, nodes, parameters);

                // Weights
                var inputWeight = inputInfo.InputWeightParam.GetParamInfo(parameters, ParamType.Float);
                inputWeights[i] = inputWeight;
            }

            return new AnimationScriptInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(parameters, ParamType.Float),
                animator.GetComponent<Skeleton>(), _animationScriptable);
        }
    }
}