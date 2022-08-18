using UnityEditor.UIElements;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationGraphValueNode : ValueNodeBase<AnimationGraphAsset, Playable>
    {
        public override AnimationGraphAsset LiteralValue => _literalValueField?.value as AnimationGraphAsset;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new ObjectField
                    {
                        objectType = typeof(AnimationGraphAsset)
                    };
                }

                return _literalValueField;
            }
        }
        private ObjectField _literalValueField;


        public AnimationGraphValueNode()
        {
            title = "Animation Graph Value";

            OutputPort.portColor = Colors.AnimationPlayableColor;

            RefreshInputView();
        }

        public override void RebuildPorts()
        {
            throw new System.NotImplementedException();
        }
    }
}