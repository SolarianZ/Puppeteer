using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationClipValueNode : ValueNodeBase<AnimationClip>
    {
        public override AnimationClip LiteralValue => _literalValueField?.value as AnimationClip;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new ObjectField
                    {
                        objectType = typeof(AnimationClip)
                    };
                }

                return _literalValueField;
            }
        }
        private ObjectField _literalValueField;


        public AnimationClipValueNode()
        {
            title = "Animation Clip Value";

            OutputPort.portColor = Colors.AnimationPlayableColor;

            RefreshInputView();
        }
    }
}