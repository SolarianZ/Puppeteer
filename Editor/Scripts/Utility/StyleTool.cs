using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using EditorUIEToolbar = UnityEditor.UIElements.Toolbar;

namespace GBG.AnimationGraph.Editor.Utility
{
    public static class StyleTool
    {
        public static StyleSheet GetCommonLightStyleSheet()
        {
            var getCommonLightStyleSheetMethod = GetUIElementsEditorUtilityMethod("GetCommonLightStyleSheet");
            var commonLightStyleSheet = (StyleSheet)getCommonLightStyleSheetMethod!.Invoke(null, null);

            return commonLightStyleSheet;
        }

        public static StyleSheet GetCommonDarkStyleSheet()
        {
            var getCommonDarkStyleSheetMethod = GetUIElementsEditorUtilityMethod("GetCommonDarkStyleSheet");
            var commonDarkStyleSheet = (StyleSheet)getCommonDarkStyleSheetMethod!.Invoke(null, null);

            return commonDarkStyleSheet;
        }

        public static void ForceDarkStyleSheet(VisualElement element)
        {
            if (EditorGUIUtility.isProSkin)
            {
                return;
            }

            var forceDarkStyleSheetMethod = GetUIElementsEditorUtilityMethod("ForceDarkStyleSheet");
            forceDarkStyleSheetMethod!.Invoke(null, new object[] { element });
        }


        private static MethodInfo GetUIElementsEditorUtilityMethod(string methodName)
        {
            var uieEditorUtilType = typeof(EditorUIEToolbar).Assembly
                .GetType("UnityEditor.UIElements.UIElementsEditorUtility");
            var methodInfo = uieEditorUtilType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo;
        }
    }
}
