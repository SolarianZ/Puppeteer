using UnityEditor.UIElements;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationScriptValueNode : ValueNodeBase<AnimationScriptPlayableAsset, Playable>
    {
        public override AnimationScriptPlayableAsset LiteralValue => _literalValueField?.value as AnimationScriptPlayableAsset;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new ObjectField
                    {
                        objectType = typeof(AnimationScriptPlayableAsset)
                    };
                }

                return _literalValueField;
            }
        }
        private ObjectField _literalValueField;


        public AnimationScriptValueNode()
        {
            title = "Animation Script Value";

            OutputPort.portColor = Colors.AnimationPlayableColor;

            RefreshInputView();
        }

        public override void RebuildPorts()
        {
            throw new System.NotImplementedException();
        }
    }
}