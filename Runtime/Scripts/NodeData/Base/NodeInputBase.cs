using System;
using UnityEngine;

namespace GBG.AnimationGraph.NodeData
{
    [Serializable]
    public abstract class NodeInputBase
    {
        public string InputNodeGuid
        {
            get => _inputNodeGuid;
            internal set => _inputNodeGuid = value;
        }

        [SerializeField]
        private string _inputNodeGuid;
    }
}
