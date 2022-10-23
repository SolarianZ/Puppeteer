using System;
using UnityEngine;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class NodeBase
    {
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


        protected NodeBase(string guid)
        {
            _guid = guid;
        }
    }
}
