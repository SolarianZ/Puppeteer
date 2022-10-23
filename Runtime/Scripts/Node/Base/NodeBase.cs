using System;
using UnityEngine;
using UnityEngine.Playables;

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
        public Vector2 EditorPosition
        {
            get => _editorPosition;
            internal set => _editorPosition = value;
        }

        [SerializeField]
        private Vector2 _editorPosition;
#endif

        #endregion


        #region Runtime Properties

        // internal abstract Playable AnimationPlayable { get; }

        #endregion


        protected NodeBase(string guid)
        {
            _guid = guid;
        }


        public void Initialize(PlayableGraph playableGraph)
        {
        }
    }
}