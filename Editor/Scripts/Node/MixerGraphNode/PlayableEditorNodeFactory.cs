using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Node;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class PlayableEditorNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipEditorNode), typeof(AnimationClipNode) },
            { typeof(AnimationMixerEditorNode), typeof(AnimationMixerNode) },
            { typeof(AnimationLayerMixerEditorNode), typeof(AnimationLayerMixerNode) },
            { typeof(StateMachineEditorNode), typeof(StateMachineNode) },
            { typeof(SubGraphEditorNode), typeof(SubGraphNode) },
            { typeof(AnimationScriptEditorNode), typeof(AnimationScriptNode) },
            { typeof(BlendSpace1DEditorNode), typeof(BlendSpace1DNode) },
            { typeof(BlendSpace2DEditorNode), typeof(BlendSpace2DNode) },
            { typeof(ScriptEditorNode), typeof(ScriptNode) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipNode), typeof(AnimationClipEditorNode) },
            { typeof(AnimationMixerNode), typeof(AnimationMixerEditorNode) },
            { typeof(AnimationLayerMixerNode), typeof(AnimationLayerMixerEditorNode) },
            { typeof(StateMachineNode), typeof(StateMachineEditorNode) },
            { typeof(SubGraphNode), typeof(SubGraphEditorNode) },
            { typeof(AnimationScriptNode), typeof(AnimationScriptEditorNode) },
            { typeof(BlendSpace1DNode), typeof(BlendSpace1DEditorNode) },
            { typeof(BlendSpace2DNode), typeof(BlendSpace2DEditorNode) },
            { typeof(ScriptNode), typeof(ScriptEditorNode) },
        };


        public static IEnumerable<Type> GetPlayableNodeTypes() => _nodeToDataType.Keys;

        public static MixerGraphEditorNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType, Vector2 position)
        {
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (NodeBase)Activator.CreateInstance(nodeDataType, GuidTool.NewGuid());
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData, true);
        }

        public static MixerGraphEditorNode CreateNode(AnimationGraphAsset graphAsset, NodeBase node,
            bool isCreateFromContextualMenu)
        {
            var nodeType = _dataToNodeType[node.GetType()];
            var extraInfo = new EditorNodeExtraInfo(isCreateFromContextualMenu);
            var editorNode = (MixerGraphEditorNode)Activator.CreateInstance(nodeType, graphAsset, node, extraInfo);
            return editorNode;
        }
    }
}
