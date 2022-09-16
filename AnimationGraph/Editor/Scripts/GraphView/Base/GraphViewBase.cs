using System;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UGraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public abstract class GraphViewBase : UGraphView
    {
        private const string _GRID_BACKGROUND_STYLE_PATH = "AnimationGraph/GridBackground";

        public string Guid => GraphData.Guid;

        public abstract GraphNode RootNode { get; }

        protected AnimationGraphAsset GraphAsset { get; }

        protected GraphData.GraphData GraphData { get; }


        protected GraphViewBase(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
        {
            GraphAsset = graphAsset;
            GraphData = graphData;

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            style.flexGrow = 1;

            // Grid background
            var gridStyleSheet = Resources.Load<StyleSheet>(_GRID_BACKGROUND_STYLE_PATH);
            styleSheets.Add(gridStyleSheet);
            var gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);

            // Callbacks
            graphViewChanged += OnGraphViewChanged;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            FrameAll();
        }


        #region View Change Event

        public bool SuppressGraphViewChangedEvent { get; set; }

        public event Action OnGraphChanged;


        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (!SuppressGraphViewChangedEvent)
            {
                OnGraphChanged?.Invoke();
            }

            return graphViewChange;
        }

        #endregion
    }
}
