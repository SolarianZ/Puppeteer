using UnityEditor;
using UnityEditor.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationGraphEditorWindow : EditorWindow
    {
        [MenuItem("Test/Animation Graph Editor Window")]
        public static AnimationGraphEditorWindow Open()
        {
            return GetWindow<AnimationGraphEditorWindow>("Animation Graph Editor");
        }


        private void OnEnable()
        {
            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            var graphView = new AnimationMixerGraphView();
            rootVisualElement.Add(graphView);
        }
    }
}
