using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
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
        internal Vector2 EditorPosition
        {
            get => _editorPosition;
            set => _editorPosition = value;
        }

        [SerializeField]
        private Vector2 _editorPosition;
#endif

        #endregion


        protected NodeBase(string guid)
        {
            _guid = guid;
        }


        internal void InitializePlayable(PlayableGraph playableGraph,
            IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
        }

        internal void InitializeConnection(IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
        }

        internal virtual void PrepareFrame(float deltaTime)
        {
        }

        // TODO: This method should be abstract
        internal Playable GetPlayable() => Playable.Null;
    }
}
