using System;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public class SubGraphNodeData : PlayableNodeData
    {
        public AnimationGraphAsset SubGraph
        {
            get => _subGraph;
            internal set => _subGraph = value;
        }

        [SerializeField]
        private AnimationGraphAsset _subGraph;


        public SubGraphNodeData(string guid) : base(guid)
        {
        }
    }
}
