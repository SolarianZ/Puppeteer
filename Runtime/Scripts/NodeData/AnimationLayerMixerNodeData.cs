using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationLayerMixerNodeData : AnimationNodeData
    {
        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            var inputInstances = new AnimationNodeInstance[InputInfos.Length];
            var inputWeights = new ParamInfo[InputInfos.Length];
            var layerAdditiveStates = new bool[InputInfos.Length];
            var layerAvatarMasks = new AvatarMask[InputInfos.Length];
            for (var i = 0; i < InputInfos.Length; i++)
            {
                var inputInfo = (LayerMixerInputInfo)InputInfos[i];

                // Inputs
                var inputNode = nodeTable[inputInfo.InputNodeGuid];
                inputInstances[i] = inputNode.CreateNodeInstance(graph, animator, nodeTable, paramTable);

                // Weights
                var inputWeight = inputInfo.InputWeightParam.GetParamInfo(paramTable, ParamType.Float);
                inputWeights[i] = inputWeight;

                // Additive states
                layerAdditiveStates[i] = inputInfo.IsAdditive;

                // AvatarMasks
                layerAvatarMasks[i] = inputInfo.AvatarMask;
            }

            return new AnimationLayerMixerInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float),
                layerAdditiveStates, layerAvatarMasks);
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationLayerMixerNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var animLayerMixerNodeData = (AnimationLayerMixerNodeData)clone;
            animLayerMixerNodeData.InputInfos = new InputInfo[InputInfos.Length];
            for (int i = 0; i < InputInfos.Length; i++)
            {
                animLayerMixerNodeData.InputInfos[i] = (LayerMixerInputInfo)InputInfos[i].Clone();
            }
        }

        #endregion
    }
}
