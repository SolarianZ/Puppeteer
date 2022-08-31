using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphView;
using GBG.Puppeteer.Graph;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow : EditorWindow
    {
        private static readonly List<AnimationGraphWindow> _openedWindows
            = new List<AnimationGraphWindow>();

        [OnOpenAsset]
        internal static bool OnOpenAnimationGraphAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is RuntimeAnimationGraph animGraphAsset)
            {
                var editor = _openedWindows.Find(window => window._graphAsset == animGraphAsset);
                if (!editor)
                {
                    editor = CreateInstance<AnimationGraphWindow>();
                    editor.titleContent = new GUIContent(animGraphAsset.name);
                    editor.SetAsset(animGraphAsset);
                }

                editor.Show();
                editor.Focus();

                return true;
            }

            return false;
        }


        private VisualElement _layoutContainer;


        private void OnEnable()
        {
            _openedWindows.Add(this);

            // Toolbar
            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            // Layout container
            _layoutContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    width = Length.Percent(100),
                    height = Length.Percent(100)
                }
            };
            rootVisualElement.Add(_layoutContainer);

            // Fill view
            CreateBlackboardPanel();
            CreateGraphViewPanel();
            CreateInspectorPanel();

            // Restore graph after code compiling,
            // but will lose all unsaved changes
            if (_graphAsset)
            {
                SetAsset(_graphAsset);
            }
        }

        private void OnDisable()
        {
            _openedWindows.Remove(this);
        }

        private void OnProjectChange()
        {
            if (!_graphAsset)
            {
                hasUnsavedChanges = false;
                Close();
                return;
            }

            titleContent = new GUIContent(_graphAsset.name);
        }
    }
}
