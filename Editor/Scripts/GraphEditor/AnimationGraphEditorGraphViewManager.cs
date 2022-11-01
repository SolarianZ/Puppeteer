using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphView;
using GBG.AnimationGraph.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public class AnimationGraphEditorGraphViewManager
    {
        public event Action<int> OnActiveGraphChanged;

        public event Action<IReadOnlyList<ISelectable>> OnGraphViewSelectionChanged;

        public event Action<DataCategories> OnDataChanged;

        public event Action OnWantsToSaveChanges;


        private AnimationGraphAsset _graphAsset;

        private readonly VisualElement _viewContainer;

        private readonly ToolbarBreadcrumbs _graphViewBreadcrumbs;

        private readonly Stack<GraphViewBase> _openedGraphViews = new Stack<GraphViewBase>();

        private readonly List<string> _openedGraphGuids = new List<string>();


        public AnimationGraphEditorGraphViewManager(AnimationGraphEditorMode mode, VisualElement viewContainer)
        {
            _viewContainer = viewContainer;

            var graphViewToolbar = new Toolbar();
            _viewContainer.Add(graphViewToolbar);
            _graphViewBreadcrumbs = new ToolbarBreadcrumbs();
            graphViewToolbar.Add(_graphViewBreadcrumbs);
        }

        public void OpenGraphView(AnimationGraphAsset graphAsset, string graphGuid, bool clearStack)
        {
            _graphAsset = graphAsset;

            if (clearStack)
            {
                CloseGraphViews(null);
            }

            if (_openedGraphGuids.Contains(graphGuid))
            {
                CloseGraphViews(graphGuid);
                return;
            }

            var activeGraphView = GetActiveGraphView();
            if (activeGraphView != null)
            {
                _viewContainer.Remove(activeGraphView);
            }

            var graphData = _graphAsset.GraphLayers.Find(graph => graph.Guid.Equals(graphGuid));
            GraphViewBase graphView = graphData.GraphType == GraphType.StateMachine
                ? new StateMachineGraphView(_graphAsset, graphData)
                : new MixerGraphView(_graphAsset, graphData);
            graphView.OnContentChanged += OnDataChanged;
            graphView.OnSelectionChanged += OnGraphViewSelectionChanged;
            graphView.OnWantsToSaveChanges += OnWantsToSaveChanges;
            graphView.OnWantsToOpenGraph += OnWantsToOpenGraph;

            _viewContainer.Add(graphView);
            _openedGraphViews.Push(graphView);
            _openedGraphGuids.Add(graphData.Guid);
            _graphViewBreadcrumbs.PushItem(graphData.Name, () => { CloseGraphViews(graphData.Guid); });
            _graphViewBreadcrumbs[_graphViewBreadcrumbs.childCount - 1].style.overflow = Overflow.Hidden;

            // Notify active graph changed
            OnActiveGraphChanged?.Invoke(_graphAsset.GraphLayers.IndexOf(graphData));

            // Clear inspector target
            OnGraphViewSelectionChanged?.Invoke(null);
        }

        public void CloseGraphViews(string stopAtGuid)
        {
            for (int i = _openedGraphGuids.Count - 1; i >= 0; i--)
            {
                var graph = _openedGraphViews.Peek();
                if (graph.Guid.Equals(stopAtGuid))
                {
                    _viewContainer.Add(graph);
                    return;
                }

                _openedGraphViews.Pop();
                _openedGraphGuids.RemoveAt(i);
                _graphViewBreadcrumbs.PopItem();

                if (_viewContainer.Contains(graph))
                {
                    _viewContainer.Remove(graph);
                }
            }
        }

        public void RefreshBreadcrumbs()
        {
            Assert.AreEqual(_graphViewBreadcrumbs.childCount, _openedGraphGuids.Count);

            var openedGraphViews = _openedGraphViews.ToArray();
            for (int i = 0; i < _openedGraphGuids.Count; i++)
            {
                var graphName = openedGraphViews[i].Name;
                ((TextElement)_graphViewBreadcrumbs[i]).text = graphName;
            }
        }

        public IReadOnlyList<ISelectable> GetSelectedGraphElements()
        {
            return GetActiveGraphView()?.selection;
        }

        public void FrameAll()
        {
            var activeGraphView = GetActiveGraphView();
            activeGraphView?.FrameAll();
        }

        // TODO: Update graph view
        public void Update(DataCategories changedDataCategories)
        {
        }

        public string[] GetOpenedGraphGuids()
        {
            return _openedGraphGuids.ToArray();
        }


        private GraphViewBase GetActiveGraphView()
        {
            return _openedGraphViews.Count > 0 ? _openedGraphViews.Peek() : null;
        }

        private void OnWantsToOpenGraph(string graphGuid)
        {
            OpenGraphView(_graphAsset, graphGuid, false);
        }
    }
}
