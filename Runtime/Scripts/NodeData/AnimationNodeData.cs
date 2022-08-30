using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public abstract class AnimationNodeData
    {
        #region EDITOR DATA

#if UNITY_EDITOR
        internal string EditorName
        {
            get => _editorName;
            set => _editorName = value;
        }

        [SerializeField]
        private string _editorName;


        internal Vector2 EditorPosition
        {
            get => _editorPosition;
            set => _editorPosition = value;
        }

        [SerializeField]
        private Vector2 _editorPosition;
#endif

        #endregion

        public string Guid
        {
            get => _guid;
            internal set => _guid = value;
        }

        [SerializeField]
        private string _guid;


        protected ParamNameOrValue PlaybackSpeed => _playbackSpeed;

        [SerializeField]
        private ParamNameOrValue _playbackSpeed;


        public abstract AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters);


        public AnimationNodeData Clone(string newGuid)
        {
            var clone = InternalDeepClone();
            clone._guid = newGuid;
            return clone;
        }

        protected abstract AnimationNodeData InternalDeepClone();
    }
}