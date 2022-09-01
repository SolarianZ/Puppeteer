using System;
using GBG.Puppeteer.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public sealed class ParamElement : VisualElement
    {
        public TextField ParamName { get; }

        public FloatField ParamFloatValue { get; }

        public IntegerField ParamIntValue { get; }

        public Toggle ParamBoolValue { get; }

        public ParamInfo ParamInfo { get; private set; }


        public event Action<ParamElement> OnParamChanged;


        private readonly Label _paramTypeLabel;


        public ParamElement()
        {
            // Type
            _paramTypeLabel = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                }
            };
            Add(_paramTypeLabel);

            // Name
            ParamName = new TextField
            {
                style =
                {
                    flexGrow = 1,
                    maxWidth = 92,
                }
            };
            ParamName.RegisterValueChangedCallback(OnParamNameValueChanged);
            Add(ParamName);

            // Float value
            ParamFloatValue = new FloatField
            {
                style =
                {
                    width = 48,
                }
            };
            ParamFloatValue.RegisterValueChangedCallback(OnFloatValueChanged);

            // Int value
            ParamIntValue = new IntegerField
            {
                style =
                {
                    width = 48,
                }
            };
            ParamIntValue.RegisterValueChangedCallback(OnIntValueChanged);

            // Bool value
            ParamBoolValue = new Toggle();
            ParamBoolValue.RegisterValueChangedCallback(OnBoolValueChanged);
        }

        public void PopulateView(ParamInfo paramInfo)
        {
            style.flexDirection = FlexDirection.Row;

            Assert.IsFalse(string.IsNullOrEmpty(paramInfo.Name));
            ParamInfo = paramInfo;

            // Type
            _paramTypeLabel.text = ParamInfo.Type.ToString().Substring(0, 1);

            // Name
            ParamName.value = ParamInfo.Name;

            // Value
            // Remove all value fields
            if (Contains(ParamFloatValue)) Remove(ParamFloatValue);
            if (Contains(ParamIntValue)) Remove(ParamIntValue);
            if (Contains(ParamBoolValue)) Remove(ParamBoolValue);

            // Add value field and set value
            if (ParamInfo.Type == ParamType.Float)
            {
                ParamFloatValue.value = ParamInfo.GetFloat();
                Add(ParamFloatValue);
            }
            else if (ParamInfo.Type == ParamType.Int)
            {
                ParamIntValue.value = ParamInfo.GetInt();
                Add(ParamIntValue);
            }
            else if (ParamInfo.Type == ParamType.Bool)
            {
                ParamBoolValue.value = ParamInfo.GetBool();
                ParamBoolValue.value = ParamInfo.GetBool();
                Add(ParamBoolValue);
            }
            else
            {
                throw new ArgumentException($"[Puppeteer::Parameter] Unknown parameter type: {ParamInfo.Type}."
                    , nameof(paramInfo));
            }
        }

        private void OnParamNameValueChanged(ChangeEvent<string> _)
        {
            ParamInfo.Name = ParamName.value;
            OnParamChanged?.Invoke(this);
        }

        private void OnFloatValueChanged(ChangeEvent<float> _)
        {
            ParamInfo.SetFloat(ParamFloatValue.value);
            OnParamChanged?.Invoke(this);
        }

        private void OnIntValueChanged(ChangeEvent<int> _)
        {
            ParamInfo.SetInt(ParamIntValue.value);
            OnParamChanged?.Invoke(this);
        }

        private void OnBoolValueChanged(ChangeEvent<bool> _)
        {
            ParamInfo.SetBool(ParamBoolValue.value);
            OnParamChanged?.Invoke(this);
        }
    }
}
