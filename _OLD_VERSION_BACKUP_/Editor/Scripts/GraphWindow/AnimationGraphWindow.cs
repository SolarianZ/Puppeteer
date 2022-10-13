using System.Collections.Generic;
using GBG.Puppeteer.Graph;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow : EditorWindow
    {
        private static readonly List<AnimationGraphWindow> _openedWindows
            = new List<AnimationGraphWindow>();

        private static AnimationGraphWindow _focusedWindow;

        [OnOpenAsset]
        public static bool OnOpenAnimationGraphAsset(int instanceId, int line)
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

        public static bool ShowNotificationOnFocusedWindow(string message, MessageType messageType, float duration)
        {
            if (!_focusedWindow)
            {
                return false;
            }

            GUIContent content;
            switch (messageType)
            {
                case MessageType.None:
                case MessageType.Info:
                    content = EditorGUIUtility.IconContent("console.infoicon");
                    break;

                case MessageType.Warning:
                    content = EditorGUIUtility.IconContent("console.warnicon");
                    break;

                case MessageType.Error:
                    content = EditorGUIUtility.IconContent("console.erroricon");
                    break;

                default:
                    return false;
            }

            content.text = message;
            _focusedWindow.ShowNotification(content);

            return true;
        }


        private TripleSplitterRowView _layoutContainer;


        private void OnEnable()
        {
            _openedWindows.Add(this);

            // Toolbar
            CreateToolbar();

            // Layout container
            _layoutContainer = new TripleSplitterRowView(new Vector2(200, 400), new Vector2(200, 400));
            rootVisualElement.Add(_layoutContainer);

            // Fill view
            CreateBlackboardPanel();
            CreateGraphViewPanel();
            CreateInspectorPanel();

            // On create view end
            // Hide inspector by default
            ToggleInspectorPanel();

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

        private void OnFocus()
        {
            _focusedWindow = this;
        }

        private void OnLostFocus()
        {
            if (_focusedWindow == this)
            {
                _focusedWindow = null;
            }
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