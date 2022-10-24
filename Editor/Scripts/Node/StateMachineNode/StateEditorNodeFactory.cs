using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Node;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class StateEditorNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(StateEditorNode), typeof(StateNode) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(StateNode), typeof(StateEditorNode) },
        };


        public static IEnumerable<Type> GetStateNodeTypes() => _nodeToDataType.Keys;

        public static StateEditorNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType,
            GraphType graphType, Vector2 position, out Graph.GraphLayer graphLayer)
        {
            graphLayer = new Graph.GraphLayer(GuidTool.NewGuid(),
                $"SubGraph_{GuidTool.NewUniqueSuffix()}", graphType);
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (StateNode)Activator.CreateInstance(nodeDataType, graphLayer.Guid);
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData, graphLayer);
        }

        public static StateEditorNode CreateNode(AnimationGraphAsset graphAsset, StateNode node,
            Graph.GraphLayer graphLayer)
        {
            var nodeType = _dataToNodeType[node.GetType()];
            var editorNode = (StateEditorNode)Activator.CreateInstance(nodeType, graphAsset, node, graphLayer);
            editorNode.title = graphLayer.Name;

            return editorNode;
        }
    }
}
