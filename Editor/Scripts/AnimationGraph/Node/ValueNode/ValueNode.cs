using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public enum ValueSource
    {
        Literal,

        Parameter
    }

    public class InputField<TValue> : VisualElement
    {
        public TValue Value
        {
            get => _inputField.value;
            set => _inputField.value = value;
        }
        private readonly TextInputBaseField<TValue> _inputField;

        private readonly Label _label;


        public InputField(string label, TextInputBaseField<TValue> inputField,
            FlexDirection flexDirection = FlexDirection.Row,
            Length? labelWidth = null, Length? inputFieldMinWidth = null)
        {
            style.flexDirection = flexDirection;
            style.width = new Length(100, LengthUnit.Percent);
            style.height = new Length(100, LengthUnit.Percent);

            _label = new Label(label)
            {
                style =
                {
                    marginLeft = 4
                }
            };
            if (labelWidth != null)
            {
                _label.style.width = labelWidth.Value;
            }
            Add(_label);

            _inputField = inputField;
            _inputField.label = null;
            if (inputFieldMinWidth != null)
            {
                _inputField.style.minWidth = inputFieldMinWidth.Value;
            }
            Add(_inputField);
        }
    }

    public class ValueNode : AnimationGraphNode
    {
        public ValueSource Source { get; protected set; } = ValueSource.Literal;

        public string ParameterName
        {
            get => _parameterNameField.Value;
            private set => _parameterNameField.Value = value;
        }
        private readonly InputField<string> _parameterNameField = new InputField<string>("Parameter Name",
            new TextField(), FlexDirection.Column);

        public float FloatValue
        {
            get => _floatField.Value;
            private set => _floatField.Value = value;
        }
        private readonly InputField<float> _floatField = new InputField<float>("Float", new FloatField(),
            FlexDirection.Row, _literalLabelWidth, _literalInputMinWidth);

        public int IntValue
        {
            get => _intField.Value;
            private set => _intField.Value = value;
        }
        private readonly InputField<int> _intField = new InputField<int>("Integer", new IntegerField(),
            FlexDirection.Row, _literalLabelWidth, _literalInputMinWidth);

        public string StringValue
        {
            get => _stringField.Value;
            private set => _stringField.Value = value;
        }
        private readonly InputField<string> _stringField = new InputField<string>("String", new TextField(),
            FlexDirection.Row, _literalLabelWidth, _literalInputMinWidth);


        private static readonly Length _literalLabelWidth = new Length(48, LengthUnit.Pixel);

        private static readonly Length _literalInputMinWidth = new Length(48, LengthUnit.Pixel);


        public ValueNode() : this(new ValueNodeData()
        {
            Title = "Value Node",
            Guid = NewGuid(),
            IsRootNode = false,
            TypeAssemblyQualifiedName = typeof(ValueNode).AssemblyQualifiedName,
        })
        {
        }

        public ValueNode(ValueNodeData nodeData) : base(nodeData)
        {
            var isRebuild = NodeData.Ports.Count > 0;

            // float
            var floatOutputPort = InstantiatePort(Direction.Output,
                typeof(float), FindPortGuid<float>(Direction.Output, !isRebuild));
            floatOutputPort.portName = "Float";
            outputContainer.Add(floatOutputPort);
            var floatOutputPortData = (PortData)floatOutputPort.userData;
            Ports.Add(floatOutputPortData.Guid, floatOutputPort);
            if (!isRebuild)
            {
                NodeData.Ports.Add(floatOutputPortData);
            }
            // int
            var intOutputPort = InstantiatePort(Direction.Output,
                typeof(int), FindPortGuid<int>(Direction.Output, !isRebuild));
            intOutputPort.portName = "Integer";
            outputContainer.Add(intOutputPort);
            var intOutputPortData = (PortData)intOutputPort.userData;
            Ports.Add(intOutputPortData.Guid, intOutputPort);
            if (!isRebuild)
            {
                NodeData.Ports.Add(intOutputPortData);
            }
            // string
            var stringOutputPort = InstantiatePort(Direction.Output,
                typeof(string), FindPortGuid<string>(Direction.Output, !isRebuild));
            stringOutputPort.portName = "String";
            outputContainer.Add(stringOutputPort);
            var stringOutputPortData = (PortData)stringOutputPort.userData;
            Ports.Add(stringOutputPortData.Guid, stringOutputPort);
            if (!isRebuild)
            {
                NodeData.Ports.Add(stringOutputPortData);
            }

            // input
            var sourcePopup = new EnumField(Source);
            sourcePopup.RegisterCallback<ChangeEvent<Enum>>(OnSourceChanged);
            inputContainer.Add(sourcePopup);
            _parameterNameField.RegisterCallback<ChangeEvent<string>>(OnParamNameChanged);
            inputContainer.Add(_parameterNameField);
            _floatField.RegisterCallback<ChangeEvent<float>>(OnFloatValueChanged);
            inputContainer.Add(_floatField);
            _intField.RegisterCallback<ChangeEvent<int>>(OnIntValueChanged);
            inputContainer.Add(_intField);
            _stringField.RegisterCallback<ChangeEvent<string>>(OnStringValueChanged);
            inputContainer.Add(_stringField);

            RefreshInputView();

            RefreshExpandedState();
            RefreshPorts();
        }

        public override void RebuildPorts()
        {
            var valueNodeData = (ValueNodeData)NodeData;
            Source = valueNodeData.Source;
            ParameterName = valueNodeData.ParameterName;
            FloatValue = valueNodeData.FloatValue;
            IntValue = valueNodeData.IntValue;
            StringValue = valueNodeData.StringValue;

            RefreshInputView();
        }


        private void OnSourceChanged(ChangeEvent<Enum> evt)
        {
            Source = (ValueSource)evt.newValue;
            RefreshInputView();
        }

        private void OnParamNameChanged(ChangeEvent<string> evt)
        {
            var valueNodeData = (ValueNodeData)NodeData;
            valueNodeData.ParameterName = evt.newValue;
        }

        private void OnFloatValueChanged(ChangeEvent<float> evt)
        {
            var valueNodeData = (ValueNodeData)NodeData;
            valueNodeData.FloatValue = evt.newValue;
        }

        private void OnIntValueChanged(ChangeEvent<int> evt)
        {
            var valueNodeData = (ValueNodeData)NodeData;
            valueNodeData.IntValue = evt.newValue;
        }

        private void OnStringValueChanged(ChangeEvent<string> evt)
        {
            var valueNodeData = (ValueNodeData)NodeData;
            valueNodeData.StringValue = evt.newValue;
        }

        private void RefreshInputView()
        {
            switch (Source)
            {
                case ValueSource.Literal:
                    if (inputContainer.Contains(_parameterNameField))
                    {
                        inputContainer.Remove(_parameterNameField);
                    }

                    inputContainer.Add(_floatField);
                    inputContainer.Add(_intField);
                    inputContainer.Add(_stringField);
                    break;

                case ValueSource.Parameter:
                    if (inputContainer.Contains(_floatField))
                    {
                        inputContainer.Remove(_floatField);
                        inputContainer.Remove(_intField);
                        inputContainer.Remove(_stringField);
                    }
                    inputContainer.Add(_parameterNameField);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string FindPortGuid<TPortValueType>(Direction dir, bool allowNull)
        {
            var portData = NodeData.Ports.Find(
                    p => p.Direction == dir &&
                         Type.GetType(p.TypeAssemblyQualifiedName) == typeof(TPortValueType)
                );

            if (portData != null)
            {
                return portData.Guid;
            }

            Assert.IsTrue(allowNull);
            return null;
        }
    }
}