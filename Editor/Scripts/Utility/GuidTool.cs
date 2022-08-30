using UnityEditor;

namespace GBG.Puppeteer.Editor.Utility
{
    public static class GuidTool
    {
        public static string NewGuid()
        {
            return GUID.Generate().ToString();
        }
    }
}
