using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphView;
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


        private void OnEnable()
        {
            _openedWindows.Add(this);
        }

        private void OnDisable()
        {
            _openedWindows.Remove(this);
        }


        private AnimationGraphView _graphView;
    }
}
