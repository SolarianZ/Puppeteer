using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public enum ValueSource
    {
        Literal,

        Parameter
    }

    public abstract class ValueNodeBase<TValueType> : AnimationGraphNode
    {
        public ValueSource Source { get; protected set; } = ValueSource.Literal;

        public abstract TValueType LiteralValue { get; }

        public string ParameterName => _parameterNameField?.value;
        private TextField _parameterNameField;

        protected abstract VisualElement LiteralValueField { get; }

        protected Port OutputPort { get; }


        protected ValueNodeBase() : base(false)
        {
            var sourcePopup = new EnumField(Source);
            sourcePopup.RegisterCallback<ChangeEvent<Enum>>(OnSourceChanged);
            inputContainer.Add(sourcePopup);

            OutputPort = InstantiatePort(Direction.Output, typeof(TValueType));
            outputContainer.Add(OutputPort);

            RefreshExpandedState();
            RefreshPorts();
        }


        protected virtual void OnSourceChanged(ChangeEvent<Enum> evt)
        {
            Source = (ValueSource)evt.newValue;
            RefreshInputView();
        }


        protected void RefreshInputView()
        {
            switch (Source)
            {
                case ValueSource.Literal:
                    if (_parameterNameField != null)
                    {
                        inputContainer.Remove(_parameterNameField);
                    }

                    inputContainer.Add(LiteralValueField);
                    break;

                case ValueSource.Parameter:
                    if (LiteralValueField != null)
                    {
                        inputContainer.Remove(LiteralValueField);
                    }

                    if (_parameterNameField == null)
                    {
                        _parameterNameField = new TextField();
                    }

                    inputContainer.Add(_parameterNameField);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}