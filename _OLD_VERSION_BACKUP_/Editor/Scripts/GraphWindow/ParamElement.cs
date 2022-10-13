using System;
using System.Text;
using System.Text.RegularExpressions;
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
            style.flexDirection = FlexDirection.Row;

            // Type
            _paramTypeLabel = new Label
            {
                style =
                {
                    fontSize = 12,
                    width = 12,
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
                    flexShrink = 1,
                }
            };
            ParamName.RegisterValueChangedCallback(OnParamNameValueChanged);
            Add(ParamName);

            // Float value
            ParamFloatValue = new FloatField
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            ParamFloatValue.RegisterValueChangedCallback(OnFloatValueChanged);

            // Int value
            ParamIntValue = new IntegerField
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            ParamIntValue.RegisterValueChangedCallback(OnIntValueChanged);

            // Bool value
            ParamBoolValue = new Toggle
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            ParamBoolValue.RegisterValueChangedCallback(OnBoolValueChanged);
        }

        public void PopulateView(ParamInfo paramInfo)
        {
            Assert.IsFalse(string.IsNullOrEmpty(paramInfo.Name));
            ParamInfo = paramInfo;

            // Type
            _paramTypeLabel.text = ParamInfo.Type.ToString().Substring(0, 1);

            // Name
            ParamName.SetValueWithoutNotify(ParamInfo.Name);

            // Value
            // Remove all value fields
            if (Contains(ParamFloatValue)) Remove(ParamFloatValue);
            if (Contains(ParamIntValue)) Remove(ParamIntValue);
            if (Contains(ParamBoolValue)) Remove(ParamBoolValue);

            // Add value field and set value
            if (ParamInfo.Type == ParamType.Float)
            {
                ParamFloatValue.SetValueWithoutNotify(ParamInfo.GetFloat());
                Add(ParamFloatValue);
            }
            else if (ParamInfo.Type == ParamType.Int)
            {
                ParamIntValue.SetValueWithoutNotify(ParamInfo.GetInt());
                Add(ParamIntValue);
            }
            else if (ParamInfo.Type == ParamType.Bool)
            {
                ParamBoolValue.SetValueWithoutNotify(ParamInfo.GetBool());
                Add(ParamBoolValue);
            }
            else
            {
                throw new ArgumentException($"[Puppeteer::Parameter] Unknown parameter type: {ParamInfo.Type}."
                    , nameof(paramInfo));
            }
        }

        private void OnParamNameValueChanged(ChangeEvent<string> evt)
        {
            // Check param name
            var newName = evt.newValue;
            if (!Regex.IsMatch(newName, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                var nameBuilder = new StringBuilder(evt.newValue.Length);
                for (var i = 0; i < evt.newValue.Length; i++)
                {
                    var ch = evt.newValue[i];
                    if ((ch >= 'a' && ch <= 'z') ||
                        (ch >= 'A' && ch <= 'Z') ||
                        (ch >= '0' && ch <= '9' && nameBuilder.Length != 0) ||
                        ch == '_')
                    {
                        nameBuilder.Append(ch);
                    }
                }

                if (nameBuilder.Length == 0)
                {
                    // Use old name
                    ParamName.SetValueWithoutNotify(ParamInfo.Name);
                }
                else
                {
                    // Use new name
                    ParamName.SetValueWithoutNotify(nameBuilder.ToString());
                }

                Debug.LogError("[Puppeteer::Param] " +
                               $"Rename invalid parameter name '{evt.newValue}' to '{ParamName.value}'.");
            }
            else
            {
                ParamInfo.EditorSetName(ParamName.value);
            }

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
