using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class IntValueNode : ValueNodeBase<int>
    {
        public override int LiteralValue => _literalValueField?.value ?? default;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new IntegerField();
                }

                return _literalValueField;
            }
        }
        private IntegerField _literalValueField;


        public IntValueNode()
        {
            title = "Int Value";

            RefreshInputView();
        }
    }
}