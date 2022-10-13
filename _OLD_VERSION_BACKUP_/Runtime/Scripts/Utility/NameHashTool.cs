using UnityEngine;

namespace GBG.Puppeteer
{
    public static class NameHashTool
    {
        public static int StringToHash(string name)
        {
            return Animator.StringToHash(name);
        }
    }
}