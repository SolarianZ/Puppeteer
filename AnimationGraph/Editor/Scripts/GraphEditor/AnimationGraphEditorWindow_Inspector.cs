using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private AnimationGraphEditorInspectorManager _inspectorManager;


        private void CreateInspectorPanel()
        {
            _inspectorManager = new AnimationGraphEditorInspectorManager(_editorMode, _layoutContainer.RightPane);
            _inspectorManager.OnDataChanged += OnDataChanged;
        }

        private void SetInspectTarget(IReadOnlyList<ISelectable> selection)
        {
            _inspectorManager.SetInspectTarget(selection);
        }
    }
}
