using System;
using System.Collections.Generic;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;
using UObject = UnityEngine.Object;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationScriptNodeData : AnimationNodeData
    {
        public AnimationScriptableObject AnimationScriptable
        {
            get => _animationScriptable;
            set => _animationScriptable = value;
        }

        [SerializeField]
        private AnimationScriptableObject _animationScriptable;

        public ParamBindingNameOrValue[] ParamBindingSources
        {
            get => _paramBindingSources;
            set => _paramBindingSources = value;
        }

        [SerializeField]
        private ParamBindingNameOrValue[] _paramBindingSources;


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
                animator.GetComponent<Skeleton>(), UObject.Instantiate(_animationScriptable));
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
            animScriptNodeData.AnimationScriptable = AnimationScriptable;
            
            animScriptNodeData.ParamBindingSources = new ParamBindingNameOrValue[ParamBindingSources.Length];
            for (int i = 0; i < ParamBindingSources.Length; i++)
            {
                animScriptNodeData.ParamBindingSources[i] = (ParamBindingNameOrValue)ParamBindingSources[i].Clone();
            }
            
            animScriptNodeData.InputInfos = new InputInfo[InputInfos.Length];
            for (int i = 0; i < InputInfos.Length; i++)
            {
                animScriptNodeData.InputInfos[i] = (MixerInputInfo)InputInfos[i].Clone();
            }
        }

        #endregion
    }
}