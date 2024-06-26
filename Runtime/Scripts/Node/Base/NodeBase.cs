﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;
using UPlayableExtensions = UnityEngine.Playables.PlayableExtensions;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class NodeBase
    {
        #region Serialization Data

        public string Guid => _guid;

        [SerializeField]
        private string _guid;


#if UNITY_EDITOR
        internal Vector2 EditorPosition
        {
            get => _editorPosition;
            set => _editorPosition = value;
        }

        [SerializeField]
        private Vector2 _editorPosition;
#endif

        #endregion


        #region Runtime Properties

        protected static string[] EmptyInputs = Array.Empty<string>();

        protected internal Playable Playable { get; private set; }

        protected NodeBase[] RuntimeInputNodes { get; private set; }


        private bool _isPaused;

        private double _speedCache;

        #endregion


        protected NodeBase(string guid)
        {
            _guid = guid;
        }

        
        #region Lifecycle

        internal void InitializeData(Animator animator, PlayableGraph playableGraph,
            IReadOnlyDictionary<string, ParamInfo> paramGuidTable,
            IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, AnimationGraphAsset> externalGraphGuidTable)
        {
            InitializeGraphLink(graphGuidTable, externalGraphGuidTable);
            InitializeParams(paramGuidTable);
            Playable = CreatePlayable(animator, playableGraph);
        }

        protected virtual void InitializeGraphLink(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, AnimationGraphAsset> externalGraphGuidTable)
        {
        }

        protected abstract void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable);

        protected abstract Playable CreatePlayable(Animator animator, PlayableGraph playableGraph);
        
        
        protected internal virtual void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            var inputGuids = GetInputNodeGuids();
            RuntimeInputNodes = new NodeBase[inputGuids.Count];

            for (int i = 0; i < inputGuids.Count; i++)
            {
                var inputGuid = inputGuids[i];
                if (string.IsNullOrEmpty(inputGuid))
                {
                    continue;
                }

                var inputNode = nodeGuidTable[inputGuid];
                RuntimeInputNodes[i] = inputNode;
                Playable.ConnectInput(i, inputNode.Playable, 0);
            }
        }

        protected internal abstract IReadOnlyList<string> GetInputNodeGuids();

        
        protected internal abstract void PrepareFrame(FrameData frameData);

        
        protected internal virtual void Dispose()
        {
        }

        #endregion


        #region Speed & Play & Pause // See APIMask.cs

        protected internal double GetSpeed()
        {
            if (_isPaused)
            {
                return _speedCache;
            }

            return UPlayableExtensions.GetSpeed(Playable);
        }

        protected internal void SetSpeed(double speed)
        {
            _speedCache = speed;
            if (!_isPaused)
            {
                UPlayableExtensions.SetSpeed(Playable, speed);
            }
        }

        protected internal void Play()
        {
            if (_isPaused)
            {
                _isPaused = false;
                UPlayableExtensions.SetSpeed(Playable, _speedCache);
            }
        }

        protected internal void Pause()
        {
            if (!_isPaused)
            {
                _isPaused = true;
                _speedCache = UPlayableExtensions.GetSpeed(Playable);
                UPlayableExtensions.SetSpeed(Playable, 0);
            }
        }

        #endregion
    }
}
