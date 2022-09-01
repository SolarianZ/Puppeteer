using System;
using System.Collections.Generic;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationScriptNodeData : AnimationNodeData
    {
        [SerializeField]
        private AnimationScriptableObject _animationScriptable;


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

                // Inputs
                var inputNode = nodeTable[inputInfo.InputNodeGuid];
                inputInstances[i] = inputNode.CreateNodeInstance(graph, animator, nodeTable, paramTable);

                // Weights
                var inputWeight = inputInfo.InputWeightParam.GetParamInfo(paramTable, ParamType.Float);
                inputWeights[i] = inputWeight;
            }

            return new AnimationScriptInstance(graph, inputInstances, inputWeights,
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float),
                animator.GetComponent<Skeleton>(), _animationScriptable);
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationScriptNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var animScriptNodeData = (AnimationScriptNodeData)clone;
            animScriptNodeData.InputInfos = new InputInfo[InputInfos.Length];
            for (int i = 0; i < InputInfos.Length; i++)
            {
                animScriptNodeData.InputInfos[i] = (MixerInputInfo)InputInfos[i].Clone();
            }
        }

        #endregion
    }
}
