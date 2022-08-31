using System.Collections.Generic;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public delegate bool AnimationGraphNodeCreator(AnimationNodeData nodeData, out PlayableNode node);

    public static class PlayableNodeFactory
    {
        private static AnimationGraphNodeCreator _customNodeCreator;

        public static void SetCustomNodeCreator(AnimationGraphNodeCreator nodeCreator)
        {
            _customNodeCreator = nodeCreator;
        }


        public static PlayableNode InstantiateNode(AnimationNodeData nodeData, List<ParamInfo> parameters)
        {
            PlayableNode node = null;
            if (_customNodeCreator != null && _customNodeCreator(nodeData, out node))
            {
                return node;
            }

            // TODO: Create node according to data type
            var nodeDataType = nodeData.GetType();
            if (nodeDataType == typeof(AnimationClipNodeData))
            {
                node = new AnimationClipNode(nodeData.Guid);
            }
            // ...
            else // Unknown node data type
            {
                Debug.LogError("[Puppeteer::AnimationNodeFactory] Unknown node data type: " +
                               $"{nodeDataType.AssemblyQualifiedName}.");
            }

            if (node != null)
            {
                node.title = nodeData.EditorName;
                node.SetPosition(new Rect(nodeData.EditorPosition, Vector2.zero));
                node.PopulateView(nodeData, parameters);
            }

            return node;
        }
    }
}
