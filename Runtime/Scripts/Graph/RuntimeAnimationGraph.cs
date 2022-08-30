using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Graph
{
    [CreateAssetMenu(fileName = nameof(RuntimeAnimationGraph),
        menuName = "Puppeteer/[TEST] Runtime Animation Graph")]
    public class RuntimeAnimationGraph : ScriptableObject
    {
        public IReadOnlyList<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private ParamInfo[] _parameters = Array.Empty<ParamInfo>();

        
        // Node at index 0 is always the output node
        public IReadOnlyList<AnimationNodeData> Nodes => _nodes;

        [SerializeReference]
        private AnimationNodeData[] _nodes = Array.Empty<AnimationNodeData>();


        // FOR TEST
        private void Reset()
        {
            _nodes = new AnimationNodeData[]
            {
                new AnimationClipNodeData(),
                new AnimationMixerNodeData(),
            };
        }
    }
}