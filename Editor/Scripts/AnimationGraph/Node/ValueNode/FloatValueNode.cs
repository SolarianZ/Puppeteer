using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class FloatValueNode : ValueNodeBase<float>
    {
        public override float LiteralValue => _literalValueField?.value ?? default;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new FloatField();
                }

                return _literalValueField;
            }
        }
        private FloatField _literalValueField;


        public FloatValueNode()
        {
            title = "Float Value";

            RefreshInputView();
        }
    }
}