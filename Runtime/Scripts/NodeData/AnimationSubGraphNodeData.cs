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
        [SerializeField]
        private RuntimeAnimationGraph _subGraph;

        [SerializeField]
        private ParamBindingNameOrValue[] _paramBindingSources;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            if (_paramBindingSources == null || _paramBindingSources.Length == 0)
            {
                return new AnimationSubGraphInstance(graph, animator, _subGraph, null);
            }

            var paramBindingSources = new ParamInfo[_subGraph.Parameters.Count];
            for (int i = 0; i < paramBindingSources.Length; i++)
            {
                foreach (var bindingSource in _paramBindingSources)
                {
                    if (bindingSource.TargetParamName.Equals(_subGraph.Parameters[i].Name))
                    {
                        paramBindingSources[i] = bindingSource
                            .GetParamBindingSource(paramTable, _subGraph.Parameters[i].Type);
                        break;
                    }
                }
            }

            return new AnimationSubGraphInstance(graph, animator, _subGraph, paramBindingSources);
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
            animSubGraphNodeData._subGraph = _subGraph;

            animSubGraphNodeData._paramBindingSources = new ParamBindingNameOrValue[_paramBindingSources.Length];
            for (int i = 0; i < _paramBindingSources.Length; i++)
            {
                animSubGraphNodeData._paramBindingSources[i] = (ParamBindingNameOrValue)_paramBindingSources[i].Clone();
            }
        }

        #endregion
    }
}
