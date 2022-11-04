using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GBG.AnimationGraph
{
    [Serializable]
    public class AnimationGraphAssetLink
    {
        public string Guid => _guid;

        [SerializeField]
        private string _guid;


        public AnimationGraphAsset GraphAsset
        {
            get => _graphAsset;
            internal set => _graphAsset = value;
        }

        [SerializeField]
        private AnimationGraphAsset _graphAsset;


        public AnimationGraphAssetLink(string guid)
        {
            _guid = guid;
        }
    }

    // [CreateAssetMenu(fileName = nameof(AnimationGraphAsset),
    //     menuName = "Puppeteer/[TEST] Animation Graph")]
    public class AnimationGraphAsset : ScriptableObject, IDisposable
    {
        #region Serialization Data

        public List<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private List<ParamInfo> _parameters = new List<ParamInfo>();


        public string RootGraphGuid
        {
            get => _rootGraphGuid;
            internal set => _rootGraphGuid = value;
        }

        [SerializeField]
        private string _rootGraphGuid;


        public List<GraphLayer> GraphLayers => _graphLayers;

        [SerializeField]
        private List<GraphLayer> _graphLayers = new List<GraphLayer>();


        public List<AnimationGraphAssetLink> ExternalGraphs => _externalGraphs;

        [SerializeField]
        private List<AnimationGraphAssetLink> _externalGraphs = new List<AnimationGraphAssetLink>();


#if UNITY_EDITOR

        // TODO: Isolated graph layers(Editor only) 
        public List<GraphLayer> IsolatedGraphLayers => _isolatedGraphLayers;

        [SerializeField]
        private List<GraphLayer> _isolatedGraphLayers = new List<GraphLayer>();


        public string EditorGuid
        {
            get => _editorGuid;
            private set => _editorGuid = value;
        }

        [SerializeField]
        private string _editorGuid;
#endif

        #endregion


        #region Runtime Properties

        public GraphLayer RuntimeRootGraph { get; private set; }

        public NodeBase RuntimeRootNode => RuntimeRootGraph?.RuntimeRootNode;


        private Dictionary<string, ParamInfo> _paramNameTable;

        #endregion


        public ParamInfo FindParameterByGuid(string paramGuid)
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.Guid.Equals(paramGuid))
                {
                    return parameter;
                }
            }

            return null;
        }

        public void Initialize(PlayableGraph playableGraph, Dictionary<string, GraphLayer> outGraphGuidTable)
        {
            // Variables
            _paramNameTable = new Dictionary<string, ParamInfo>(Parameters.Count);

            if (string.IsNullOrEmpty(RootGraphGuid))
            {
                return;
            }

            // Internal graph layers
            foreach (var graphLayer in GraphLayers)
            {
                outGraphGuidTable.Add(graphLayer.Guid, graphLayer);
                if (RuntimeRootGraph == null && graphLayer.Guid.Equals(RootGraphGuid))
                {
                    RuntimeRootGraph = graphLayer;
                }
            }

            // External graph layers
            var externalGraphGuidTable = new Dictionary<string, AnimationGraphAsset>(ExternalGraphs.Count);
            foreach (var externalGraph in ExternalGraphs)
            {
                if (externalGraph.GraphAsset)
                {
                    externalGraph.GraphAsset = Instantiate(externalGraph.GraphAsset);
                    externalGraph.GraphAsset.Initialize(playableGraph, outGraphGuidTable);
                }

                externalGraphGuidTable.Add(externalGraph.Guid, externalGraph.GraphAsset);
            }

            // Parameters
            var paramGuidTable = new Dictionary<string, ParamInfo>(Parameters.Count);
            foreach (var paramInfo in Parameters)
            {
                _paramNameTable.Add(paramInfo.Name, paramInfo);
                paramGuidTable.Add(paramInfo.Guid, paramInfo);
            }

            // Nodes(Playables)
            foreach (var graphLayer in GraphLayers)
            {
                graphLayer.InitializeNodes(playableGraph, paramGuidTable,
                    outGraphGuidTable, externalGraphGuidTable);
            }

            // Connections
            foreach (var graphLayer in GraphLayers)
            {
                graphLayer.InitializeConnections();
            }
        }

        public void Dispose()
        {
            // External graph layers
            foreach (var externalGraph in ExternalGraphs)
            {
                if (externalGraph.GraphAsset)
                {
                    externalGraph.GraphAsset.Dispose();
                    Destroy(externalGraph.GraphAsset);
                }
            }

            // Internal graph layers
            foreach (var graphLayer in GraphLayers)
            {
                graphLayer.Dispose();
            }
        }


        #region Parameters

        public float GetFloat(string paramName)
        {
            return _paramNameTable[paramName].GetFloat();
        }

        public int GetInt(string paramName)
        {
            return _paramNameTable[paramName].GetInt();
        }

        public bool GetBool(string paramName)
        {
            return _paramNameTable[paramName].GetBool();
        }

        public float GetRawValue(string paramName)
        {
            return _paramNameTable[paramName].RawValue;
        }

        public void SetFloat(string paramName, float value)
        {
            _paramNameTable[paramName].SetFloat(value);
        }

        public void SetInt(string paramName, int value)
        {
            _paramNameTable[paramName].SetInt(value);
        }

        public void SetBool(string paramName, bool value)
        {
            _paramNameTable[paramName].SetBool(value);
        }

        public void SetRawValue(string paramName, float value)
        {
            _paramNameTable[paramName].SetRawValue(value);
        }

        #endregion


        #region Editor Debug

#if UNITY_EDITOR

        private static readonly Dictionary<string, AnimationGraphAsset> _editorInstances =
            new Dictionary<string, AnimationGraphAsset>();

        public static AnimationGraphAsset EditorGetInstance(string graphAssetGuid)
        {
            return _editorInstances[graphAssetGuid];
        }


        private void Reset()
        {
            EditorGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        }

        private void Awake()
        {
            _editorInstances[EditorGuid] = this;
        }

        private void OnDestroy()
        {
            _editorInstances.Remove(EditorGuid);
        }

#endif

        #endregion
    }
}
