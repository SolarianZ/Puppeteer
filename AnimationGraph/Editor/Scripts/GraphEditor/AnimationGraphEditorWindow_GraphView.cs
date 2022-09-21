using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphView;
using GBG.AnimationGraph.GraphData;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private ToolbarBreadcrumbs _graphViewBreadcrumbs;

        private readonly Stack<GraphViewBase> _openedGraphViews = new Stack<GraphViewBase>();

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // Ensure this filed is serializable(don't need SerializeField attribute) 
        // as thus we can keep this field's value after re-compile codes
        private List<string> _openedGraphGuids = new List<string>();

        private GraphViewBase ActiveGraphView => _openedGraphViews.Count > 0 ? _openedGraphViews.Peek() : null;


        private void CreateGraphViewPanel()
        {
            var graphViewToolbar = new Toolbar();
            _layoutContainer.MiddlePane.Add(graphViewToolbar);

            _graphViewBreadcrumbs = new ToolbarBreadcrumbs();
            graphViewToolbar.Add(_graphViewBreadcrumbs);
        }


        private void OpenGraphView(string graphGuid, bool clearStack)
        {
            if (clearStack)
            {
                CloseGraphViews(null);
            }

            if (_openedGraphGuids.Contains(graphGuid))
            {
                CloseGraphViews(graphGuid);
                return;
            }

            if (ActiveGraphView != null)
            {
                _layoutContainer.MiddlePane.Remove(ActiveGraphView);
            }

            var graphData = Graphs.Find(graph => graph.Guid.Equals(graphGuid));
            GraphViewBase graphView = graphData.GraphType == GraphType.StateMachine
                ? new StateMachineGraphView(_graphAsset, graphData)
                : new MixerGraphView(_graphAsset, graphData);
            graphView.OnGraphViewChanged += OnGraphViewContentChanged;
            graphView.OnSelectionChanged += SetInspectTarget;
            _layoutContainer.MiddlePane.Add(graphView);
            _openedGraphViews.Push(graphView);
            _openedGraphGuids.Add(graphData.Guid);
            _graphViewBreadcrumbs.PushItem(graphData.Name, () => { CloseGraphViews(graphData.Guid); });
            _graphViewBreadcrumbs[_graphViewBreadcrumbs.childCount - 1].style.overflow = Overflow.Hidden;
            _graphListView.SetSelection(_graphAsset.Graphs.IndexOf(graphData));

            SetInspectTarget(null);
        }

        private void CloseGraphViews(string stopAtGuid)
        {
            var isActiveGraph = true;
            for (int i = _openedGraphGuids.Count - 1; i >= 0; i--)
            {
                var graph = _openedGraphViews.Peek();
                if (graph.Guid.Equals(stopAtGuid))
                {
                    return;
                }

                _openedGraphViews.Pop();
                _openedGraphGuids.RemoveAt(i);
                _graphViewBreadcrumbs.PopItem();

                if (isActiveGraph)
                {
                    _layoutContainer.MiddlePane.Remove(graph);
                    isActiveGraph = false;
                }
            }

            if (!isActiveGraph && ActiveGraphView != null)
            {
                _layoutContainer.MiddlePane.Add(ActiveGraphView);
            }
        }

        private void RestoreGraphViews()
        {
            if (_openedGraphGuids.Count == 0)
            {
                return;
            }

            var graphGuids = _openedGraphGuids.ToArray();
            _openedGraphViews.Clear();
            _openedGraphGuids.Clear();

            for (int i = 0; i < graphGuids.Length; i++)
            {
                var graphGuid = graphGuids[i];
                OpenGraphView(graphGuid, false);
            }
        }

        private void RefreshGraphViewBreadcrumbLabel(string graphGuid, string graphName)
        {
            Assert.AreEqual(_graphViewBreadcrumbs.childCount, _openedGraphGuids.Count);

            for (int i = 0; i < _openedGraphGuids.Count; i++)
            {
                if (_openedGraphGuids[i].Equals(graphGuid))
                {
                    ((TextElement)_graphViewBreadcrumbs[i]).text = graphName;
                    break;
                }
            }
        }


        private void OnGraphViewContentChanged()
        {
            hasUnsavedChanges = true;
        }
    }
}
