using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationBlendSpace1DNodeData : AnimationNodeData
    {
        public ParamNameOrValue Position
        {
            get => _position;
            set => _position = value;
        }

        [SerializeField]
        private ParamNameOrValue _position = new ParamNameOrValue(null, 0);

        public MotionField1D[] MotionFields
        {
            get => _motionFields;
            set => _motionFields = value;
        }

        [SerializeField]
        private MotionField1D[] _motionFields = Array.Empty<MotionField1D>();


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            return new BlendSpace1DInstance(graph, MotionFields,
                Position.GetParamInfo(paramTable, ParamType.Bool),
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationBlendSpace1DNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var blendSpace1DNodeData = (AnimationBlendSpace1DNodeData)clone;
            blendSpace1DNodeData._position = (ParamNameOrValue)_position.Clone();
            blendSpace1DNodeData._motionFields = new MotionField1D[MotionFields.Length];
            for (int i = 0; i < MotionFields.Length; i++)
            {
                blendSpace1DNodeData._motionFields[i] = (MotionField1D)MotionFields[i].Clone();
            }
        }

        #endregion
    }
}
