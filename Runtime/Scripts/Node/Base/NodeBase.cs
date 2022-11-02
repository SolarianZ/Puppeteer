using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
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


        #region Runtime Properties

        protected static string[] EmptyInputs = Array.Empty<string>();

        protected internal Playable Playable { get; private set; }

        #endregion


        protected NodeBase(string guid)
        {
            _guid = guid;
        }


        protected internal abstract IList<string> GetInputNodeGuids();

        internal void InitializeData(PlayableGraph playableGraph,
            IReadOnlyDictionary<string, ParamInfo> paramGuidTable)
        {
            InitializeParams(paramGuidTable);
            Playable = CreatePlayable(playableGraph);
        }

        protected internal virtual void InitializeConnection(IReadOnlyDictionary<string, GraphLayer> graphGuidTable,
            IReadOnlyDictionary<string, NodeBase> nodeGuidTable)
        {
            var inputGuids = GetInputNodeGuids();
            for (int i = 0; i < inputGuids.Count; i++)
            {
                var inputGuid = inputGuids[i];
                if (string.IsNullOrEmpty(inputGuid))
                {
                    continue;
                }

                var inputNode = nodeGuidTable[inputGuid];
                Playable.ConnectInput(i, inputNode.Playable, 0, GetInputWeight(i));
            }
        }

        protected internal abstract void PrepareFrame(float deltaTime);


        protected abstract void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable);

        protected abstract Playable CreatePlayable(PlayableGraph playableGraph);

        protected abstract float GetInputWeight(int inputIndex);
    }
}
