using System;
using System.Collections.Generic;
using GBG.Puppeteer.Node;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Graph
{
    public class RuntimeAnimationStateMachineGraph : ScriptableObject
    {
        public IReadOnlyList<StateMachineStateNode> Nodes => _nodes;

        [SerializeField]
        private StateMachineStateNode[] _nodes = Array.Empty<StateMachineStateNode>();


        public IReadOnlyList<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private ParamInfo[] _parameters = Array.Empty<ParamInfo>();
    }
}