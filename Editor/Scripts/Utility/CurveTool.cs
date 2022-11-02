using UnityEngine;

namespace GBG.AnimationGraph.Editor.Utility
{
    public static class CurveTool
    {
        public static bool IsConstant(this AnimationCurve curve)
        {
            if (curve.length == 0)
            {
                return true;
            }

            var value = curve[0].value;
            for (int i = 1; i < curve.length; i++)
            {
                if (!Mathf.Approximately(value, curve[i].value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNormalized(this AnimationCurve curve)
        {
            return curve.length == 2 &&
                   Mathf.Approximately(curve[0].time, 0) &&
                   Mathf.Approximately(curve[0].value, 0) &&
                   Mathf.Approximately(curve[1].time, 1) &&
                   Mathf.Approximately(curve[1].value, 1);
        }

        public static void Normalize(this AnimationCurve curve)
        {
            if (curve.IsConstant())
            {
                curve.ClearKeys();
                curve.AddKey(new Keyframe(0, 0, 0, 1));
                curve.AddKey(new Keyframe(1, 1, 1, 0));
                return;
            }

            var startKey = curve[0];
            var endKey = curve[curve.length - 1];
            var timeOffset = startKey.time - 0f;
            var timeScale = 1.0f / (endKey.time - startKey.time);
            var valueOffset = startKey.value - 0f;
            var valueScale = 1.0f / (endKey.value - startKey.value);
            var keys = curve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                ref var key = ref keys[i];
                key.time = (key.time - timeOffset) * timeScale;
                key.value = (key.value - valueOffset) * valueScale;
            }

            curve.keys = keys;
        }

        public static void ClearKeys(this AnimationCurve curve)
        {
            for (int i = curve.length - 1; i >= 0; i--)
            {
                curve.RemoveKey(i);
            }
        }
    }
}
