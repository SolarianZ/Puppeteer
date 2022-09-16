using System;
using UnityEditor;

namespace GBG.AnimationGraph.Editor.Utility
{
    public static class GuidTool
    {
        #region GUID

        private static Func<string> _customGuidGenerator;


        public static void SetCustomGuidGenerator(Func<string> customGuidGenerator)
        {
            _customGuidGenerator = customGuidGenerator;
        }

        public static string NewGuid()
        {
            if (_customGuidGenerator != null)
            {
                return _customGuidGenerator();
            }

            return GUID.Generate().ToString();
        }

        public static string NewUniqueSuffix()
        {
            return Math.Abs(NewGuid().GetHashCode()).ToString();
        }

        #endregion
    }
}
