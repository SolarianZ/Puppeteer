using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationMixerNodeData : AnimationNodeData
    {
        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator,
            Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            var inputInstances = new AnimationNodeInstance[InputInfos.Length];
            var inputWeights = new ParamInfo[InputInfos.Length];
            for (var i = 0; i < InputInfos.Length; i++)
            {
                var inputInfo = (MixerInputInfo)InputInfos[i];
                if (inputInfo == null)
                {
                    continue;
                }

                // Inputs
                var inputNode = nodeTable[inputInfo.InputNodeGuid];
                inputInstances[i] = inputNode.CreateNodeInstance(graph, animator, nodeTable, paramTable);

                // Weights
                var inputWeight = inputInfo.InputWeightParam.GetParamInfo(paramTable, ParamType.Float);
                inputWeights[i] = inputWeight;
            }

            return new AnimationMixerInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationMixerNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var animMixerNodeData = (AnimationMixerNodeData)clone;
            animMixerNodeData.InputInfos = new InputInfo[InputInfos.Length];
            for (int i = 0; i < InputInfos.Length; i++)
            {
                animMixerNodeData.InputInfos[i] = (MixerInputInfo)InputInfos[i].Clone();
            }
        }

        #endregion
    }
}
