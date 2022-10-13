using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public abstract class AnimationNodeData
    {
        #region EDITOR DATA

#if UNITY_EDITOR
        public string EditorName
        {
            get => _editorName;
            set => _editorName = value;
        }

        [SerializeField]
        private string _editorName;


        public Vector2 EditorPosition
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
            set => _guid = value;
        }

        [SerializeField]
        private string _guid;

        public InputInfo[] InputInfos
        {
            get => _inputInfos;
            set => _inputInfos = value;
        }

        [SerializeReference]
        private InputInfo[] _inputInfos;

        public ParamNameOrValue PlaybackSpeed
        {
            get => _playbackSpeed;
            set => _playbackSpeed = value;
        }

        [SerializeField]
        private ParamNameOrValue _playbackSpeed = new ParamNameOrValue(null, 1);


        public abstract AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodeTable,
            Dictionary<string, ParamInfo> paramTable);


        #region Deep Clone

        public AnimationNodeData Clone(string newGuid)
        {
            var clone = CreateCloneInstance();
            clone._guid = newGuid;
            CloneMembers(clone);

            return clone;
        }


        protected abstract AnimationNodeData CreateCloneInstance();

        protected virtual void CloneMembers(AnimationNodeData clone)
        {
#if UNITY_EDITOR
            clone.EditorName = _editorName;
            clone._editorPosition = _editorPosition;
#endif
            clone._playbackSpeed = (ParamNameOrValue)_playbackSpeed.Clone();
        }

        #endregion
    }
}
