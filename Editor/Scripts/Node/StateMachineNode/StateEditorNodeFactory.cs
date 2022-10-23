﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class StateEditorNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(StateEditorNode), typeof(StateNodeData) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(StateNodeData), typeof(StateEditorNode) },
        };


        public static IEnumerable<Type> GetStateNodeTypes() => _nodeToDataType.Keys;

        public static StateEditorNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType,
            GraphType graphType, Vector2 position, out GraphData.GraphData nodeGraphData)
        {
            nodeGraphData = new GraphData.GraphData(GuidTool.NewGuid(),
                $"SubGraph_{GuidTool.NewUniqueSuffix()}", graphType);
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (StateNodeData)Activator.CreateInstance(nodeDataType, nodeGraphData.Guid);
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData, nodeGraphData);
        }

        public static StateEditorNode CreateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData,
            GraphData.GraphData graphData)
        {
            var nodeType = _dataToNodeType[nodeData.GetType()];
            var node = (StateEditorNode)Activator.CreateInstance(nodeType, graphAsset, nodeData, graphData);
            node.title = graphData.Name;

            return node;
        }
    }
}