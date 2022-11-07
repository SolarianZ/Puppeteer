using UnityEngine;

namespace GBG.AnimationGraph.Utility
{
    public static class NameHashTool
    {
        public static int StringToHash(string name)
        {
            return Animator.StringToHash(name);
        }
    }
}