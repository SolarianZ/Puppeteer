using UnityEngine;

namespace GBG.Puppeteer
{
    public static class TimeTool
    {
        public static float GetFixedTime(float time, TimeMode timeMode, AnimationClip clip)
        {
            if (timeMode == TimeMode.NormalizedTime && clip)
            {
                if (Mathf.Abs(time - 1) > Mathf.Epsilon)
                {
                    time %= 1;
                }
                time *= clip.length;
            }

            return time;
        }

    }
}
