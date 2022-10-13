using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GBG.AnimationGraph
{
    // [CreateAssetMenu(fileName = nameof(AnimationGraphAsset),
    //     menuName = "Puppeteer/[TEST] Animation Graph")]
    public class AnimationGraphAsset : ScriptableObject
    {
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


        public List<GraphData.GraphData> Graphs => _graphs;

        [SerializeField]
        private List<GraphData.GraphData> _graphs = new List<GraphData.GraphData>();


#if UNITY_EDITOR
        // TODO: Isolated nodes(Editor only) 
#endif


        #region Editor Debug

#if UNITY_EDITOR

        public string EditorGuid
        {
            get => _editorGuid;
            private set => _editorGuid = value;
        }

        [SerializeField]
        private string _editorGuid;


        private static readonly Dictionary<string, AnimationGraphAsset> _editorInstances =
            new Dictionary<string, AnimationGraphAsset>();

        public static AnimationGraphAsset GetInstance(string graphAssetGuid)
        {
            return _editorInstances[graphAssetGuid];
        }


        private void Reset()
        {
            EditorGuid = GUID.Generate().ToString();
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
