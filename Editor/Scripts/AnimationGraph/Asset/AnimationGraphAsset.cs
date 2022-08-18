using GBG.Puppeteer.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [CreateAssetMenu(fileName = nameof(AnimationGraph),
        menuName = "Puppeteer/Animation Graph")]
    public class AnimationGraphAsset : RuntimeAnimationGraphAsset
    {
        // nodes
        public List<NodeData> Nodes => _nodes;
        [SerializeReference]
        private List<NodeData> _nodes = new List<NodeData>();

        // ports
        public List<PortData> Ports => _ports;
        [SerializeField]
        private List<PortData> _ports = new List<PortData>();

        // edges
        public List<EdgeData> Edges => _edges;
        [SerializeField]
        private List<EdgeData> _edges = new List<EdgeData>();
    }
}