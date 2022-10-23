using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class PlayableEditorNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipEditorNode), typeof(AnimationClipNodeData) },
            { typeof(AnimationMixerEditorNode), typeof(AnimationMixerNodeData) },
            { typeof(AnimationLayerMixerEditorNode), typeof(AnimationLayerMixerNodeData) },
            { typeof(StateMachineEditorNode), typeof(StateMachineNodeData) },
            { typeof(SubGraphEditorNode), typeof(SubGraphNodeData) },
            { typeof(AnimationScriptEditorNode), typeof(AnimationScriptNodeData) },
            { typeof(BlendSpace1DEditorNode), typeof(BlendSpace1DNodeData) },
            { typeof(BlendSpace2DEditorNode), typeof(BlendSpace2DNodeData) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipNodeData), typeof(AnimationClipEditorNode) },
            { typeof(AnimationMixerNodeData), typeof(AnimationMixerEditorNode) },
            { typeof(AnimationLayerMixerNodeData), typeof(AnimationLayerMixerEditorNode) },
            { typeof(StateMachineNodeData), typeof(StateMachineEditorNode) },
            { typeof(SubGraphNodeData), typeof(SubGraphEditorNode) },
            { typeof(AnimationScriptNodeData), typeof(AnimationScriptEditorNode) },
            { typeof(BlendSpace1DNodeData), typeof(BlendSpace1DEditorNode) },
            { typeof(BlendSpace2DNodeData), typeof(BlendSpace2DEditorNode) },
        };


        public static IEnumerable<Type> GetPlayableNodeTypes() => _nodeToDataType.Keys;

        public static MixerGraphEditorNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType, Vector2 position)
        {
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (PlayableNodeData)Activator.CreateInstance(nodeDataType, GuidTool.NewGuid());
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData, true);
        }

        public static MixerGraphEditorNode CreateNode(AnimationGraphAsset graphAsset, PlayableNodeData nodeData,
            bool isCreateFromContextualMenu)
        {
            var nodeType = _dataToNodeType[nodeData.GetType()];
            var extraInfo = new EditorNodeExtraInfo(isCreateFromContextualMenu);
            var node = (MixerGraphEditorNode)Activator.CreateInstance(nodeType, graphAsset, nodeData, extraInfo);
            return node;
        }
    }
}
