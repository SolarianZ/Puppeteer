using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public partial class AnimationGraphEditorWindow
    {
        #region Key Events

        private void OnKeyDown(KeyDownEvent evt)
        {
            PressKey(evt.keyCode);

            if (IsKeysPressed(KeyCode.LeftControl, KeyCode.S) ||
                IsKeysPressed(KeyCode.RightControl, KeyCode.S))
            {
                SaveAsset();
            }
        }

        private void OnKeyUp(KeyUpEvent evt)
        {
            ReleaseKey(evt.keyCode);
        }

        #endregion


        #region Key Set

        private readonly HashSet<int> _keySet = new HashSet<int>();


        private bool IsKeyPressed(KeyCode key)
        {
            return _keySet.Contains((int)key);
        }

        private bool IsKeysPressed(KeyCode key0, KeyCode key1)
        {
            return IsKeyPressed(key0) && IsKeyPressed(key1);
        }

        private bool IsKeysPressed(params KeyCode[] keys)
        {
            foreach (var key in keys)
            {
                if (!IsKeyPressed(key))
                {
                    return false;
                }
            }

            return true;
        }

        private void PressKey(KeyCode key)
        {
            _keySet.Add((int)key);
        }

        private void ReleaseKey(KeyCode key)
        {
            _keySet.Remove((int)key);
        }

        #endregion
    }
}