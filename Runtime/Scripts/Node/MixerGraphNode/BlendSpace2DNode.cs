﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace2DSample
    {
        public AnimationClip Clip
        {
            get => _clip;
            internal set => _clip = value;
        }

        [SerializeField]
        private AnimationClip _clip;


        public Vector2 Position
        {
            get => _position;
            internal set => _position = value;
        }

        [SerializeField]
        private Vector2 _position;


        public float Speed
        {
            get => _speed;
            internal set => _speed = value;
        }

        [SerializeField]
        private float _speed = 1.0f;

        // [SerializeField]
        // public bool Mirror;
    }

    [Serializable]
    public class BlendSpace2DNode : AnimationAssetPlayerNodeBase
    {
        #region Serialization Data

        public ParamGuidOrValue PositionXParam
        {
            get => _positionXParam;
            internal set => _positionXParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionXParam = new ParamGuidOrValue(null, 0);

        public ParamGuidOrValue PositionYParam
        {
            get => _positionYParam;
            internal set => _positionYParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionYParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace2DSample> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace2DSample> _samples = new List<BlendSpace2DSample>();

        public int[] Triangles
        {
            get => _triangles;
            internal set => _triangles = value;
        }

        [SerializeField]
        private int[] _triangles = Array.Empty<int>();

        #endregion


        #region Runtime Properties

        public override FrameData FrameData { get; protected set; }


        private bool _runtimeSpeedDirty;

        private bool _runtimeMotionTimeDirty;

        #endregion


        public BlendSpace2DNode(string guid) : base(guid)
        {
        }


        #region Lifecycle

        // TODO: InitializeParams
        protected override void InitializeAssetPlayerParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable) =>
            throw new NotImplementedException();

        protected override Playable CreatePlayable(Animator animator, PlayableGraph playableGraph)
        {
            var playable = AnimationMixerPlayable.Create(playableGraph, Samples.Count);
            return playable;
        }


        protected internal override IReadOnlyList<string> GetInputNodeGuids() => EmptyInputs;

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();

        #endregion


        // TODO: GetUnscaledAnimationLength
        public override double GetUnscaledAnimationLength()
        {
            throw new NotImplementedException();
        }


        protected override void OnRuntimeSpeedParamChanged(ParamInfo paramInfo)
        {
            _runtimeSpeedDirty = true;
        }

        protected override void OnRuntimeMotionTimeParamChanged(ParamInfo paramInfo)
        {
            _runtimeMotionTimeDirty = true;
        }
    }
}
