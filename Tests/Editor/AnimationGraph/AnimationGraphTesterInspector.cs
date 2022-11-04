using System;
using GBG.AnimationGraph.Parameter;
using UnityEditor;
using UnityEngine;

namespace GBG.AnimationGraph.Tests.Editor
{
    [CustomEditor(typeof(AnimationGraphTester))]
    public class AnimationGraphTesterInspector : UnityEditor.Editor
    {
        private AnimationGraphTester Target => (AnimationGraphTester)target;

        private string[] _paramNames;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Target.Brain)
            {
                return;
            }

            EditorGUILayout.Space();

            var guiColor = GUI.color;
            GUI.color = Target.LiveParamEnabled ? Color.green : guiColor;
            if (GUILayout.Button("Live Param"))
            {
                Target.LiveParamEnabled = !Target.LiveParamEnabled;
            }

            GUI.color = guiColor;
            if (!Target.LiveParamEnabled)
            {
                return;
            }

            if (_paramNames == null)
            {
                _paramNames = Target.GetParamNames();
                if (_paramNames == null)
                {
                    return;
                }

                Target.ParamIndex = -1;
            }

            EditorGUI.BeginChangeCheck();
            Target.ParamIndex = EditorGUILayout.Popup("Param Name", Target.ParamIndex, _paramNames);
            if (EditorGUI.EndChangeCheck() && Target.ParamIndex > -1)
            {
                Target.ParamRawValue = Target.GetRawValue(_paramNames[Target.ParamIndex]);
            }

            if (Target.ParamIndex < 0)
            {
                return;
            }

            var paramType = Target.GetParamType(Target.ParamIndex);
            switch (paramType)
            {
                case ParamType.Float:
                    Target.ParamRawValue = EditorGUILayout.FloatField("Param Value", Target.ParamRawValue);
                    break;

                case ParamType.Int:
                    Target.ParamRawValue = EditorGUILayout.IntField("Param Value",
                        Mathf.RoundToInt(Target.ParamRawValue));
                    break;

                case ParamType.Bool:
                    Target.ParamRawValue = EditorGUILayout.Toggle("Param Value",
                        !Mathf.Approximately(Target.ParamRawValue, 0))
                        ? 1
                        : 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
