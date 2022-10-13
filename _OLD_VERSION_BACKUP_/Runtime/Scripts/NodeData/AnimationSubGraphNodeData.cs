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
    public class AnimationSubGraphNodeData : AnimationNodeData
    {
        public RuntimeAnimationGraph SubGraph
        {
            get => _subGraph;
            set => _subGraph = value;
        }

        [SerializeField]
        private RuntimeAnimationGraph _subGraph;

        public ParamBindingNameOrValue[] ParamBindingSources
        {
            get => _paramBindingSources;
            set => _paramBindingSources = value;
        }

        [SerializeField]
        private ParamBindingNameOrValue[] _paramBindingSources;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            if (ParamBindingSources == null || ParamBindingSources.Length == 0)
            {
                return new AnimationSubGraphInstance(graph, animator, SubGraph, null);
            }

            var paramBindingSources = new ParamInfo[SubGraph.Parameters.Count];
            for (int i = 0; i < paramBindingSources.Length; i++)
            {
                foreach (var bindingSource in ParamBindingSources)
                {
                    if (bindingSource.BindToName.Equals(SubGraph.Parameters[i].Name))
                    {
                        paramBindingSources[i] = bindingSource
                            .GetParamBindingSource(paramTable, SubGraph.Parameters[i].Type);
                        break;
                    }
                }
            }

            return new AnimationSubGraphInstance(graph, animator, SubGraph, paramBindingSources);
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationSubGraphNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var animSubGraphNodeData = (AnimationSubGraphNodeData)clone;
            animSubGraphNodeData.SubGraph = SubGraph;

            animSubGraphNodeData.ParamBindingSources = new ParamBindingNameOrValue[ParamBindingSources.Length];
            for (int i = 0; i < ParamBindingSources.Length; i++)
            {
                animSubGraphNodeData.ParamBindingSources[i] = (ParamBindingNameOrValue)ParamBindingSources[i].Clone();
            }
        }

        #endregion
    }
}
