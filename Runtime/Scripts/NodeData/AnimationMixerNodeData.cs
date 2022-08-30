using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class InputInfo : ICloneable
    {
        public string InputNodeGuid => _inputNodeGuid;

        [SerializeField]
        private string _inputNodeGuid;


        public ParamNameOrValue InputWeightParam => _inputWeightParam;

        [SerializeField]
        private ParamNameOrValue _inputWeightParam = new ParamNameOrValue();

        public object Clone()
        {
            var clone = InternalDeepClone();
            clone._inputNodeGuid = _inputNodeGuid;
            clone._inputWeightParam = _inputWeightParam;

            return clone;
        }

        protected virtual InputInfo InternalDeepClone()
        {
            return new InputInfo();
        }
    }

    [Serializable]
    public class AnimationMixerNodeData : AnimationNodeData
    {
        public IReadOnlyList<InputInfo> InputInfos => _inputInfos;

        [SerializeField]
        private InputInfo[] _inputInfos;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodes,
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

            return new AnimationMixerInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(parameters, ParamType.Float));
        }


        protected override AnimationNodeData InternalDeepClone()
        {
            var clone = new AnimationMixerNodeData();
            clone._inputInfos = new InputInfo[_inputInfos.Length];
            for (int i = 0; i < _inputInfos.Length; i++)
            {
                clone._inputInfos[i] = (InputInfo)_inputInfos[i].Clone();
            }

            return clone;
        }
    }
}