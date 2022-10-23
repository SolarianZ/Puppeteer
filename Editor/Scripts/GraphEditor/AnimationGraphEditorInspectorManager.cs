using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public class AnimationGraphEditorInspectorManager
    {
        public event Action<DataCategories> OnDataChanged;


        private readonly VisualElement _viewContainer;

        private IInspector _inspector;


        public AnimationGraphEditorInspectorManager(AnimationGraphEditorMode mode, VisualElement viewContainer)
        {
            _viewContainer = viewContainer;

            var toolbar = new Toolbar();
            _viewContainer.Add(toolbar);
            var inspectorLabel = new Label("Inspector");
            toolbar.Add(inspectorLabel);
        }

        public void SetInspectTarget(IReadOnlyList<ISelectable> selection)
        {
            IInspector newInspector = null;
            if (selection != null && selection.Count == 1)
            {
                if (selection[0] is GraphEditorNode graphNode)
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
                _viewContainer.Remove((VisualElement)_inspector);
                _inspector.OnDataChanged -= OnDataChanged;
                _inspector = null;
            }

            if (newInspector != null)
            {
                _inspector = newInspector;
                _inspector.OnDataChanged += OnDataChanged;
                _viewContainer.Add((VisualElement)_inspector);
            }
        }

        // TODO: Update inspector
        public void Update(DataCategories changedDataCategories)
        {
        }
    }
}
