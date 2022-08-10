using UnityEditor;
using UnityEditor.Callbacks;

namespace GBG.Puppeteer.Editor
{
    public class RichAnimationClipEditor : EditorWindow
    {
        [MenuItem("Window/Animation/Rich Animation Clip Editor")]
        public static RichAnimationClipEditor Open()
        {
            return GetWindow<RichAnimationClipEditor>("Rich Animation Clip Editor");
        }

        [OnOpenAsset]
        internal static bool OnOpenRichAnimationClipAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset is RichAnimationClip richAnimClip)
            {
                var editor = Open();
                editor.SetAsset(richAnimClip);

                return true;
            }

            return false;
        }


        internal void SetAsset(RichAnimationClip asset) { }
    }
}
