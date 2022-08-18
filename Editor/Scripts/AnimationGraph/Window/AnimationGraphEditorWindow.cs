using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public partial class AnimationGraphEditorWindow : EditorWindow
    {
        private static readonly List<AnimationGraphEditorWindow> _openedWindows
            = new List<AnimationGraphEditorWindow>();

        [OnOpenAsset]
        internal static bool OnOpenAnimationGraphAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is AnimationGraphAsset animGraphAsset)
            {
                var editor = _openedWindows.Find(window => !window._asset || window._asset == animGraphAsset);
                if (!editor)
                {
                    editor = EditorWindow.CreateInstance<AnimationGraphEditorWindow>();
                    editor.titleContent = new GUIContent(animGraphAsset.name);
                }
                editor.SetAsset(animGraphAsset);
                editor.Show();

                return true;
            }

            return false;
        }


        private AnimationGraphAsset _asset;

        private Toolbar _toolbar;

        private AnimationGraphView _graphView;


        private void OnEnable()
        {
            _openedWindows.Add(this);

            saveChangesMessage = "This window has unsaved changes. Would you like to save?";

            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            // recover view after compile
            if (_asset)
            {
                SetAsset(_asset);
            }

            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            rootVisualElement.RegisterCallback<KeyUpEvent>(OnKeyUp);
        }

        private void OnDisable()
        {
            _openedWindows.Remove(this);
        }

        private void SetAsset(AnimationGraphAsset asset)
        {
            Assert.IsNotNull(asset);

            _asset = asset;

            if (_graphView != null)
            {
                rootVisualElement.Remove(_graphView);
            }
            _graphView = new AnimationMixerGraphView(_asset);
            _graphView.RegisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);

            rootVisualElement.Add(_graphView);
        }

        private void OnGraphGeometryChanged(GeometryChangedEvent evt)
        {
            _graphView.UnregisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);

            _graphView.FrameAll();
        }
    }
}
