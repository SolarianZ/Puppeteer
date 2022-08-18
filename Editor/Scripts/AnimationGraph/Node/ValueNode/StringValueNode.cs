using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class StringValueNode : ValueNodeBase<string, string>
    {
        public override string LiteralValue => _literalValueField?.value;

        protected override VisualElement LiteralValueField
        {
            get
            {
                if (_literalValueField == null)
                {
                    _literalValueField = new TextField();
                }

                return _literalValueField;
            }
        }
        private TextField _literalValueField;


        public StringValueNode()
        {
            title = "String Value";

            RefreshInputView();
        }

        public override void RebuildPorts()
        {
            throw new System.NotImplementedException();
        }
    }
}