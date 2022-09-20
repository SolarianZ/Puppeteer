using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private GraphNodeInspector _graphNodeInspector;


        private void CreateInspectorPanel()
        {
            var toolbar = new Toolbar();
            _layoutContainer.RightPane.Add(toolbar);
            var inspectorLabel = new Label("Inspector");
            toolbar.Add(inspectorLabel);
        }

        private void SetInspectTarget(IReadOnlyList<ISelectable> selection)
        {
            GraphNodeInspector newInspector = null;
            if (selection != null && selection.Count == 1 && selection[0] is GraphNode graphNode)
            {
                newInspector = graphNode.GetInspector();
            }

            if (newInspector == _graphNodeInspector)
            {
                return;
            }

            if (_graphNodeInspector != null)
            {
                _layoutContainer.RightPane.Remove(_graphNodeInspector);
                _graphNodeInspector.OnParamChanged -= OnNodeParamChanged;
                _graphNodeInspector = null;
            }

            if (newInspector != null)
            {
                _graphNodeInspector = newInspector;
                _graphNodeInspector.OnParamChanged += OnNodeParamChanged;
                _layoutContainer.RightPane.Add(_graphNodeInspector);
            }
        }

        private void OnNodeParamChanged()
        {
            hasUnsavedChanges = true;
        }
    }
}
