using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Graph
{
    public class RuntimeAnimationStateMachineGraph : ScriptableObject
    {
        public IReadOnlyList<StateMachineStateNodeData> Nodes => _nodes;

        [SerializeField]
        private StateMachineStateNodeData[] _nodes = Array.Empty<StateMachineStateNodeData>();


        public IReadOnlyList<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private ParamInfo[] _parameters = Array.Empty<ParamInfo>();
    }
}