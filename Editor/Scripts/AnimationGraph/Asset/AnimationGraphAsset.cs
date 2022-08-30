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
        public List<NodeData> EditorNodes => _editorNodes;

        [SerializeReference]
        private List<NodeData> _editorNodes = new List<NodeData>();

        // edges
        public List<EdgeData> EditorEdges => _editorEdges;

        [SerializeField]
        private List<EdgeData> _editorEdges = new List<EdgeData>();
    }
}