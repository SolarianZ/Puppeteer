using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph
{
    [CreateAssetMenu(fileName = nameof(AnimationGraphAsset),
        menuName = "Puppeteer/[TEST] Animation Graph")]
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
    }
}
