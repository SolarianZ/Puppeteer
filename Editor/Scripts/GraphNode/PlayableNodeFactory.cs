using System;
using System.Collections.Generic;
using System.Linq;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor;
using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public delegate bool AnimationGraphNodeCreator(AnimationNodeData nodeData, out PlayableNode node);

    public static class PlayableNodeFactory
    {
        #region Create Node

        private static AnimationGraphNodeCreator _customNodeCreator;

        public static void SetCustomNodeCreator(AnimationGraphNodeCreator nodeCreator)
        {
            _customNodeCreator = nodeCreator;
        }


        private static readonly Dictionary<Type, Type> _nodeDataTypeTable = new Dictionary<Type, Type>
        {
            { typeof(AnimationClipNodeData), typeof(AnimationClipNode) },
            { typeof(AnimationMixerNodeData), typeof(AnimationMixerNode) },
            { typeof(AnimationLayerMixerNodeData), typeof(AnimationLayerMixerNode) },
        };

        private static readonly Type[] _playableNodeCtorParamTypes = new Type[] { typeof(string) };

        public static PlayableNode CreateNode(Type nodeType, Vector2 nodePosition)
        {
            var ctor = nodeType.GetConstructor(_playableNodeCtorParamTypes);
            if (ctor == null)
            {
                UDebug.LogError($"[Puppeteer::AnimationNodeFactory] {nodeType.Name} does not have " +
                                "a constructor with a single string parameter(guid).");
                return null;
            }

            var playableNodeCtorParams = new object[] { NewGuid() };
            var node = (PlayableNode)ctor.Invoke(playableNodeCtorParams);
            node.SetPosition(new Rect(nodePosition, Vector2.zero));

            return node;
        }

        public static PlayableNode CreateNode(AnimationNodeData nodeData, List<ParamInfo> paramTable)
        {
            if (_customNodeCreator != null && _customNodeCreator(nodeData, out var node))
            {
                node.title = nodeData.EditorName;
                node.SetPosition(new Rect(nodeData.EditorPosition, Vector2.zero));
                node.PopulateView(nodeData, paramTable);
                return node;
            }

            // Create node according to data type
            var nodeDataType = nodeData.GetType();
            if (!_nodeDataTypeTable.TryGetValue(nodeDataType, out var nodeType))
            {
                Debug.LogError("[Puppeteer::AnimationNodeFactory] Unknown node data type: " +
                               $"{nodeDataType.AssemblyQualifiedName}.");
                return null;
            }


            var ctor = nodeType.GetConstructor(_playableNodeCtorParamTypes);
            if (ctor == null)
            {
                UDebug.LogError($"[Puppeteer::AnimationNodeFactory] {nodeType.Name} does not have " +
                                "a constructor with a single string parameter(guid).");
                return null;
            }

            var playableNodeCtorParams = new object[] { nodeData.Guid };
            node = (PlayableNode)ctor.Invoke(playableNodeCtorParams);
            node.title = nodeData.EditorName;
            node.SetPosition(new Rect(nodeData.EditorPosition, Vector2.zero));
            node.PopulateView(nodeData, paramTable);

            return node;
        }

        #endregion


        #region Collect Node Types

        public static Type[] CollectAvailablePlayableNodeTypes()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where IsPlayableNodeType(type)
                select type).ToArray();
        }

        private static bool IsPlayableNodeType(Type type)
        {
            if (type.IsInterface || type.IsAbstract || (type.IsGenericType && !type.IsConstructedGenericType))
            {
                return false;
            }

            return typeof(PlayableNode).IsAssignableFrom(type);
        }

        #endregion


        #region GUID

        private static Func<string> _customGuidGenerator;


        public static void SetCustomGuidGenerator(Func<string> customGuidGenerator)
        {
            _customGuidGenerator = customGuidGenerator;
        }

        public static string NewGuid()
        {
            if (_customGuidGenerator != null)
            {
                return _customGuidGenerator();
            }

            return GUID.Generate().ToString();
        }

        #endregion
    }
}
