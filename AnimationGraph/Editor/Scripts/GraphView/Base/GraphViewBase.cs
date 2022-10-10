using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEditor;
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

        public string Name => GraphData.Name;

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
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (selection.Count > 0 && !selection.Contains(RootNode))
            {
                base.BuildContextualMenu(evt);
            }
        }


        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            FrameAll();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.ctrlKey && evt.keyCode == KeyCode.S)
            {
                OnWantsToSaveChanges?.Invoke();
            }
        }


        #region Graph View Events

        public event Action<DataCategories> OnContentChanged;

        public event Action<IReadOnlyList<ISelectable>> OnSelectionChanged;

        public event Action OnWantsToSaveChanges;

        public event Action<string> OnWantsToOpenGraph;


        protected void RaiseContentChangedEvent(DataCategories dataCategories)
        {
            OnContentChanged?.Invoke(dataCategories);
        }

        protected void RaiseWantsToOpenGraphEvent(string graphGuid)
        {
            OnWantsToOpenGraph?.Invoke(graphGuid);
        }


        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            OnSelectionChanged?.Invoke(selection);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            OnSelectionChanged?.Invoke(selection);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            OnSelectionChanged?.Invoke(selection);
        }

        #endregion
    }
}
