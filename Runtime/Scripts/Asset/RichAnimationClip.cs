using UnityEngine;

namespace GBG.Puppeteer
{
    [CreateAssetMenu(fileName = "New" + nameof(RichAnimationClip),
        menuName = "Animation/Puppeteer/Create New Rich Animation Clip")]
    public class RichAnimationClip : ScriptableObject
    {
        [SerializeField]
        private AnimationClip _clip;
    }
}