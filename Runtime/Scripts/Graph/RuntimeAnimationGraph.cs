using System;
using System.Collections.Generic;
using GBG.Puppeteer.Node;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Graph
{
    [CreateAssetMenu(fileName = nameof(RuntimeAnimationGraph),
        menuName = "Puppeteer/[TEST] Runtime Animation Graph")]
    public class RuntimeAnimationGraph : ScriptableObject
    {
        // Node at index 0 is always the output node
        public IReadOnlyList<AnimationNode> Nodes => _nodes;

        [SerializeReference]
        private AnimationNode[] _nodes = Array.Empty<AnimationNode>();


        public IReadOnlyList<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private ParamInfo[] _parameters = Array.Empty<ParamInfo>();


        // FOR TEST
        private void Reset()
        {
            _nodes = new AnimationNode[]
            {
                new AnimationClipNode(),
                new AnimationMixerNode(),
            };
        }
    }
}