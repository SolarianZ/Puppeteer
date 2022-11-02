using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Editor.ViewElement;
using GBG.AnimationGraph.Graph;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public enum AnimationGraphEditorMode
    {
        Editor,

        LiveDebug,

        Readonly,
    }

    [Flags]
    public enum DataCategories : uint
    {
        None = 0,

        Parameter = 1 << 0,

        GraphContent = 1 << 5,

        GraphList = 1 << 6,

        GraphData = GraphContent | GraphList,

        NodeData = 1 << 10,

        TransitionData = 1 << 15,
    }

    public interface IAnimationGraphEditorWindow
    {
        AnimationGraphEditorMode Mode { get; }

        AnimationGraphAsset GraphAsset { get; }

        uint DataVersion { get; }


        void OpenAsset(AnimationGraphAsset graphAsset, PlayableGraph liveDebugRuntimeGraph,
            IReadOnlyDictionary<string, Playable> liveDebugPlayables);

        void PingAsset();

        void MarkAssetChanged();

        void SaveAssetChanges();
    }

    public partial class AnimationGraphEditorWindow : EditorWindow
    {
        #region Global

        private static AnimationGraphEditorWindow _focusedWindow;

        [OnOpenAsset]
        public static bool OnOpenAnimationGraphAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is AnimationGraphAsset animGraphAsset)
            {
                var success = true;
                var editor = Resources.FindObjectsOfTypeAll<AnimationGraphEditorWindow>()
                    .FirstOrDefault(window => window._graphAsset == animGraphAsset);
                // var editor = windows.Find();
                if (!editor)
                {
                    editor = CreateInstance<AnimationGraphEditorWindow>();
                    try
                    {
                        editor.OpenGraphAsset(animGraphAsset);
                    }
                    catch
                    {
                        success = false;
                    }
                }

                if (success)
                {
                    editor.Show();
                    editor.Focus();
                }
                else
                {
                    editor.Close();
                }

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


        private AnimationGraphEditorMode _editorMode = AnimationGraphEditorMode.Editor;

        private TripleSplitterRowView _layoutContainer;

        private AnimationGraphAsset _graphAsset;

        private AnimationGraphAsset _graphAssetSnapshot;

        private DataCategories _changedDataCategories;


        public override void SaveChanges()
        {
            base.SaveChanges();
            _changedDataCategories = DataCategories.None;

            EditorUtility.SetDirty(_graphAsset);
            AssetDatabase.SaveAssetIfDirty(_graphAsset);

            DestroyImmediate(_graphAssetSnapshot);
            _graphAssetSnapshot = Instantiate(_graphAsset);
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            _changedDataCategories = DataCategories.None;

            _graphAssetSnapshot.name = _graphAsset.name;
            EditorUtility.CopySerializedIfDifferent(_graphAssetSnapshot, _graphAsset);
            DestroyImmediate(_graphAssetSnapshot);
        }


        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

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

        private void Update()
        {
            // Update sub components
            _blackboardManager.Update(_changedDataCategories);
            _graphViewManager.Update(_changedDataCategories);
            _inspectorManager.Update(_changedDataCategories);

            // Update inspect target
            if ((_changedDataCategories & DataCategories.GraphContent) != 0)
            {
                SetInspectTarget(_graphViewManager.GetSelectedGraphElements());
            }

            _changedDataCategories = DataCategories.None;
        }

        private void OnGUI()
        {
            // Shortcuts
            var evt = Event.current;
            if (evt.control && evt.keyCode == KeyCode.S)
            {
                SaveChanges();
            }
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;

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

        private void OnBeforeAssemblyReload()
        {
            _openedGraphGuids = _graphViewManager.GetOpenedGraphGuids();
          
            if (hasUnsavedChanges && _graphAsset)
            {
                var msg = "Unity is going to reload assemblies, " +
                          $"do you want to save your modifications to the animation graph '{_graphAsset.name}'?";
                if (EditorUtility.DisplayDialog("Warning", msg, "Save", "Discard"))
                {
                    SaveChanges();
                }
                else
                {
                    DiscardChanges();
                }
            }
        }

        private void OnDataChanged(DataCategories changedDataCategories)
        {
            _changedDataCategories |= changedDataCategories;
            hasUnsavedChanges |= _changedDataCategories != DataCategories.None;
        }

        // TODO: SetLiveDebugContext
        private void SetLiveDebugContext(Animator targetAnimator, IReadOnlyDictionary<string, Playable> playables)
        {
        }

        private void OpenGraphAsset(AnimationGraphAsset graphAsset)
        {
            _graphViewManager.CloseGraphViews(null);

            _graphAsset = graphAsset;
            _graphAssetSnapshot = Instantiate(_graphAsset);

            // Window
            titleContent.text = _graphAsset.name;

            // Blackboard
            _blackboardManager.Initialize(_graphAsset);

            // GraphView
            if (_graphAsset.GraphLayers.Count == 0)
            {
                // Add a default node
                var rootGraph = new GraphLayer(GuidTool.NewGuid(), "RootGraph", GraphType.Mixer);
                _graphAsset.RootGraphGuid = rootGraph.Guid;
                _graphAsset.GraphLayers.Add(rootGraph);
            }


            OpenGraphFromGraphList(_graphAsset.RootGraphGuid);
        }

        private void TryRestoreEditor()
        {
            if (!_graphAsset) return;
            _graphAssetSnapshot = Instantiate(_graphAsset);

            // Blackboard
            _blackboardManager.Initialize(_graphAsset);

            // GraphView
            if (_openedGraphGuids != null)
            {
                for (int i = 0; i < _openedGraphGuids.Length; i++)
                {
                    var graphGuid = _openedGraphGuids[i];
                    _graphViewManager.OpenGraphView(_graphAsset, graphGuid, false);
                }
            }
        }
    }
}
