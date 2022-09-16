using System;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public abstract class NodeDataBase
    {
        public string Guid => _guid;

        [SerializeField]
        private string _guid;

        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        [SerializeField]
        private string _name;

#if UNITY_EDITOR
        public Vector2 EditorPosition
        {
            get => _editorPosition;
            set => _editorPosition = value;
        }

        [SerializeField]
        private Vector2 _editorPosition;
#endif


        protected NodeDataBase(string guid)
        {
            _guid = guid;
        }
    }
}
