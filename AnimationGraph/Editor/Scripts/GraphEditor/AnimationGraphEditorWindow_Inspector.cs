using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private IInspector _inspector;


        private void CreateInspectorPanel()
        {
            var toolbar = new Toolbar();
            _layoutContainer.RightPane.Add(toolbar);
            var inspectorLabel = new Label("Inspector");
            toolbar.Add(inspectorLabel);
        }

        private void SetInspectTarget(IReadOnlyList<ISelectable> selection)
        {
            IInspector newInspector = null;
            if (selection != null && selection.Count == 1)
            {
                if (selection[0] is GraphNode graphNode)
                {
                    newInspector = graphNode.GetInspector();
                }
                else if (selection[0] is StateTransitionEdge transitionEdge)
                {
                    newInspector = transitionEdge.GetInspector();
                }
            }

            if (newInspector == _inspector)
            {
                return;
            }

            if (_inspector != null)
            {
                _layoutContainer.RightPane.Remove((VisualElement)_inspector);
                _inspector.OnParamChanged -= OnParamChanged;
                _inspector = null;
            }

            if (newInspector != null)
            {
                _inspector = newInspector;
                _inspector.OnParamChanged += OnParamChanged;
                _layoutContainer.RightPane.Add((VisualElement)_inspector);
            }
        }

        private void OnParamChanged()
        {
            hasUnsavedChanges = true;
        }
    }
}
