using UnityEngine;

namespace GBG.Puppeteer
{
    [CreateAssetMenu(fileName = "New" + nameof(RichAnimationClip),//+ ".richAnim",
        menuName = "Puppeteer/Create New Rich Animation Clip")]
    public class RichAnimationClip : ScriptableObject
    {
        public AnimationClip clip => _clip;


        [SerializeField]
        private AnimationClip _clip;
    }
}
