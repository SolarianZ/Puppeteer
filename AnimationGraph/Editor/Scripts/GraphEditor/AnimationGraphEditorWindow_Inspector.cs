using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private void CreateInspectorPanel()
        {
            var toolbar = new Toolbar();
            _layoutContainer.RightPane.Add(toolbar);
            var inspectorLabel = new Label("Inspector");
            toolbar.Add(inspectorLabel);
        }
    }
}
