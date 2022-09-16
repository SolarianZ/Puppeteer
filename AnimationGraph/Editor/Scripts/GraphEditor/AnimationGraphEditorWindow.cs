using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Editor.ViewElement;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.Parameter;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow : EditorWindow
    {
        #region Global

        private static readonly List<AnimationGraphEditorWindow> _openedWindows
            = new List<AnimationGraphEditorWindow>();

        private static AnimationGraphEditorWindow _focusedWindow;

        [OnOpenAsset]
        public static bool OnOpenAnimationGraphAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is AnimationGraphAsset animGraphAsset)
            {
                var editor = _openedWindows.Find(window => window._graphAsset == animGraphAsset);
                if (!editor)
                {
                    editor = CreateInstance<AnimationGraphEditorWindow>();
                    editor.SetGraphAsset(animGraphAsset);
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

        #endregion


        private List<ParamInfo> Parameters => _graphAsset.Parameters;

        private List<GraphData.GraphData> Graphs => _graphAsset.Graphs;

        private TripleSplitterRowView _layoutContainer;

        private AnimationGraphAsset _graphAsset;

        private AnimationGraphAsset _graphAssetSnapshot;


        public override void SaveChanges()
        {
            base.SaveChanges();

            EditorUtility.SetDirty(_graphAsset);
            AssetDatabase.SaveAssetIfDirty(_graphAsset);

            DestroyImmediate(_graphAssetSnapshot);
            _graphAssetSnapshot = Instantiate(_graphAsset);
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();

            _graphAssetSnapshot.name = _graphAsset.name;
            EditorUtility.CopySerializedIfDifferent(_graphAssetSnapshot, _graphAsset);
            DestroyImmediate(_graphAssetSnapshot);
        }


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

            // Try restore editor(after code compiling)
            TryRestoreEditor();
        }

        private void OnDisable()
        {
            _openedWindows.Remove(this);

            DestroyImmediate(_graphAssetSnapshot);
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

            titleContent.text = _graphAsset.name;
        }


        private void SetGraphAsset(AnimationGraphAsset graphAsset)
        {
            CloseGraphViews(null);

            _graphAsset = graphAsset;
            _graphAssetSnapshot = Instantiate(_graphAsset);

            // Window
            titleContent.text = _graphAsset.name;

            // Parameter
            _paramListView.itemsSource = Parameters;

            // Graph
            _graphListView.itemsSource = Graphs;

            // GraphView
            if (Graphs.Count == 0)
            {
                // Add a default node
                var rootGraph = new GraphData.GraphData(GuidTool.NewGuid(), "RootGraph", GraphType.StateMachine);
                _graphAsset.RootGraphGuid = rootGraph.Guid;
                Graphs.Add(rootGraph);
            }

            OpenGraphView(_graphAsset.RootGraphGuid, true);
        }

        private void TryRestoreEditor()
        {
            if (!_graphAsset) return;
            _graphAssetSnapshot = Instantiate(_graphAsset);

            // Parameter
            _paramListView.itemsSource = Parameters;

            // Graph
            _graphListView.itemsSource = Graphs;

            // GraphView
            RestoreGraphViews();
        }
    }
}
