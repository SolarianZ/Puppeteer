using GBG.Puppeteer.Graph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class AnimationGraphAsset : RuntimeAnimationGraph
    {
        public abstract Type RootNodeType { get; }


        // nodes
        public List<NodeData> Nodes => _nodes;
        [SerializeReference]
        private List<NodeData> _nodes = new List<NodeData>();

        // edges
        public List<EdgeData> Edges => _edges;
        [SerializeField]
        private List<EdgeData> _edges = new List<EdgeData>();
    }
}