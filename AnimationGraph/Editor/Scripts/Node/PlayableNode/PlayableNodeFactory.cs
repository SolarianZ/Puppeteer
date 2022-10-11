using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class PlayableNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipNode), typeof(AnimationClipNodeData) },
            { typeof(AnimationMixerNode), typeof(AnimationMixerNodeData) },
            { typeof(AnimationLayerMixerNode), typeof(AnimationLayerMixerNodeData) },
            { typeof(StateMachineNode), typeof(StateMachineNodeData) },
            { typeof(SubGraphNode), typeof(SubGraphNodeData) },
            { typeof(AnimationScriptNode), typeof(AnimationScriptNodeData) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipNodeData), typeof(AnimationClipNode) },
            { typeof(AnimationMixerNodeData), typeof(AnimationMixerNode) },
            { typeof(AnimationLayerMixerNodeData), typeof(AnimationLayerMixerNode) },
            { typeof(StateMachineNodeData), typeof(StateMachineNode) },
            { typeof(SubGraphNodeData), typeof(SubGraphNode) },
            { typeof(AnimationScriptNodeData), typeof(AnimationScriptNode) },
        };


        public static IEnumerable<Type> GetPlayableNodeTypes() => _nodeToDataType.Keys;

        public static PlayableNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType, Vector2 position)
        {
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (PlayableNodeData)Activator.CreateInstance(nodeDataType, GuidTool.NewGuid());
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData);
        }

        public static PlayableNode CreateNode(AnimationGraphAsset graphAsset, PlayableNodeData nodeData)
        {
            var nodeType = _dataToNodeType[nodeData.GetType()];
            var node = (PlayableNode)Activator.CreateInstance(nodeType, graphAsset, nodeData);
            return node;
        }
    }
}
