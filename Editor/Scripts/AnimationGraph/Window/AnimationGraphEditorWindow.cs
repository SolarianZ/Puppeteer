using System;
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
                editor.Focus();

                return true;
            }

            return false;
        }


        private AnimationGraphAsset _asset;

        private Toolbar _toolbar;

        private Button _pingGraphAssetButton;

        private AnimationGraphView _graphView;


        private void OnEnable()
        {
            _openedWindows.Add(this);

            saveChangesMessage = "This window has unsaved changes. Would you like to save?";

            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            _pingGraphAssetButton = new Button(PingGraphAsset)
            {
                text = "Ping Asset"
            };
            _toolbar.Add(_pingGraphAssetButton);

            // recover view after compile
            if (_asset)
            {
                SetAsset(_asset);
            }

            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            rootVisualElement.RegisterCallback<KeyUpEvent>(OnKeyUp);

            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void OnDisable()
        {
            _openedWindows.Remove(this);

            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void SetAsset(AnimationGraphAsset asset)
        {
            _asset = asset;
            Assert.IsNotNull(_asset);

            if (_graphView != null)
            {
                rootVisualElement.Remove(_graphView);
            }

            if (_asset.RootNodeType == typeof(AnimationMixerNode))
            {
                _graphView = new AnimationMixerGraphView(_asset);
            }
            else if (_asset.RootNodeType == typeof(AnimationLayerMixerNode))
            {
                _graphView = new AnimationLayerMixerGraphView(_asset);
            }
            else
            {
                throw new ArgumentException(
                    $"Unknown root node type: {_asset.RootNodeType.AssemblyQualifiedName}.",
                    nameof(asset));
            }

            _graphView.RegisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);

            rootVisualElement.Add(_graphView);
        }

        private void PingGraphAsset()
        {
            EditorGUIUtility.PingObject(_asset);
        }

        private void OnGraphGeometryChanged(GeometryChangedEvent evt)
        {
            _graphView.UnregisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);

            _graphView.FrameAll();
        }

        private void OnProjectChanged()
        {
            // check if current asset has been deleted
            if (!_asset)
            {
                Close();
                return;
            }

            // update window name
            titleContent = new GUIContent(_asset.name);
        }
    }
}
