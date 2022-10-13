using System;
using System.Collections.Generic;
using System.Linq;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationBlendSpace2DNodeData : AnimationNodeData
    {
        public ParamNameOrValue PositionX
        {
            get => _positionX;
            set => _positionX = value;
        }

        [SerializeField]
        private ParamNameOrValue _positionX = new ParamNameOrValue(null, 0);

        public ParamNameOrValue PositionY
        {
            get => _positionY;
            set => _positionY = value;
        }

        [SerializeField]
        private ParamNameOrValue _positionY = new ParamNameOrValue(null, 0);

        public MotionField2D[] MotionFields
        {
            get => _motionFields;
            set => _motionFields = value;
        }

        [SerializeField]
        private MotionField2D[] _motionFields = Array.Empty<MotionField2D>();

        public int[] Triangles
        {
            get => _triangles;
            set => _triangles = value;
        }

        [SerializeField]
        private int[] _triangles = Array.Empty<int>();


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph, Animator animator,
            Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable)
        {
            return new BlendSpace2DInstance(graph, MotionFields, Triangles,
                _positionX.GetParamInfo(paramTable, ParamType.Float),
                _positionY.GetParamInfo(paramTable, ParamType.Float),
                PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneInstance()
        {
            return new AnimationBlendSpace2DNodeData();
        }

        protected override void CloneMembers(AnimationNodeData clone)
        {
            base.CloneMembers(clone);

            var blendSpace2DNodeData = (AnimationBlendSpace2DNodeData)clone;
            blendSpace2DNodeData._positionX = (ParamNameOrValue)_positionX.Clone();
            blendSpace2DNodeData._positionY = (ParamNameOrValue)_positionY.Clone();
            blendSpace2DNodeData._motionFields = new MotionField2D[MotionFields.Length];
            for (int i = 0; i < MotionFields.Length; i++)
            {
                blendSpace2DNodeData._motionFields[i] = (MotionField2D)MotionFields[i].Clone();
            }

            blendSpace2DNodeData._triangles = _triangles.ToArray();
        }

        #endregion
    }
}
