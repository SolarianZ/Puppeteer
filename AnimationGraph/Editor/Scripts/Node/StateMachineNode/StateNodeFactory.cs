using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class StateNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(StateNode), typeof(StateNodeData) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(StateNodeData), typeof(StateNode) },
        };


        public static IEnumerable<Type> GetStateNodeTypes() => _nodeToDataType.Keys;

        public static StateNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType,
            GraphType graphType, Vector2 position)
        {
            var graphData = new GraphData.GraphData(GuidTool.NewGuid(),
                $"SubGraph_{GuidTool.NewUniqueSuffix()}", graphType);
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (StateNodeData)Activator.CreateInstance(nodeDataType, graphData);
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData);
        }

        public static StateNode CreateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData)
        {
            var nodeType = _dataToNodeType[nodeData.GetType()];
            var node = (StateNode)Activator.CreateInstance(nodeType, graphAsset, nodeData);
            node.title = nodeData.StateName;

            return node;
        }
    }
}
