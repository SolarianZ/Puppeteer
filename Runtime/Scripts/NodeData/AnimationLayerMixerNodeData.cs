using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class LayerInputInfo : InputInfo
    {
        public bool IsAdditive => _isAdditive;

        [SerializeField]
        private bool _isAdditive;


        public AvatarMask AvatarMask => _avatarMask;

        [SerializeField]
        private AvatarMask _avatarMask;


        protected override InputInfo InternalDeepClone()
        {
            return new LayerInputInfo()
            {
                _isAdditive = this._isAdditive,
                _avatarMask = this._avatarMask
            };
        }
    }

    [Serializable]
    public class AnimationLayerMixerNodeData : AnimationNodeData
    {
        public IReadOnlyList<LayerInputInfo> InputInfos => _inputInfos;

        [SerializeField]
        private LayerInputInfo[] _inputInfos;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            var inputInstances = new AnimationNodeInstance[InputInfos.Count];
            var inputWeights = new ParamInfo[InputInfos.Count];
            var layerAdditiveStates = new bool[InputInfos.Count];
            var layerAvatarMasks = new AvatarMask[InputInfos.Count];
            for (var i = 0; i < InputInfos.Count; i++)
            {
                var inputInfo = InputInfos[i];

                // Inputs
                var inputNode = nodes[inputInfo.InputNodeGuid];
                inputInstances[i] = inputNode.CreateNodeInstance(graph, animator, nodes, parameters);

                // Weights
                var inputWeight = inputInfo.InputWeightParam.GetParamInfo(parameters, ParamType.Float);
                inputWeights[i] = inputWeight;

                // Additive states
                layerAdditiveStates[i] = InputInfos[i].IsAdditive;

                // AvatarMasks
                layerAvatarMasks[i] = InputInfos[i].AvatarMask;
            }

            return new AnimationLayerMixerInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(parameters, ParamType.Float),
                layerAdditiveStates, layerAvatarMasks);
        }


        protected override AnimationNodeData InternalDeepClone()
        {
            var clone = new AnimationLayerMixerNodeData();
            clone._inputInfos = new LayerInputInfo[_inputInfos.Length];
            for (int i = 0; i < _inputInfos.Length; i++)
            {
                clone._inputInfos[i] = (LayerInputInfo)_inputInfos[i].Clone();
            }

            return clone;
        }
    }
}