using System;
using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Editor.GraphParam
{
    public class ParamField<TValue> : VisualElement //BaseField<TValue>
        where TValue : struct
    {
        public bool IsLinkedToParam { get; private set; }


        private readonly Label _label;

        private readonly Image _paramLinkImage;

        private readonly VisualElement _paramContainer;

        private readonly BaseField<TValue> _valueField;

        private readonly PopupField<ParamInfo> _paramPopup;

        private ParamInfo _linkedParam;


        public ParamField(string label, Length? labelWidth = null)
        {
            // Style
            style.flexDirection = FlexDirection.Row;

            // Label
            if (!string.IsNullOrEmpty(label))
            {
                _label = new Label(label)
                {
                    style =
                    {
                        marginLeft = 2,
                        unityTextAlign = TextAnchor.MiddleLeft,
                    }
                };

                if (labelWidth != null)
                {
                    _label.style.width = labelWidth.Value;
                }

                Add(_label);
            }

            // Param link image
            _paramLinkImage = new Image
            {
                name = "param-link-icon",
                image = GetParamLinkIcon(IsLinkedToParam)
            };
            _paramLinkImage.RegisterCallback<MouseDownEvent>(OnClickParamLinkImage);
            Add(_paramLinkImage);

            // Param container
            _paramContainer = new VisualElement()
            {
                name = "param-container",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                }
            };
            Add(_paramContainer);

            // Value field
            var valueType = typeof(TValue);
            if (valueType == typeof(float))
            {
                _valueField = new FloatField
                {
                    style =
                    {
                        flexGrow = 1
                    }
                } as BaseField<TValue>;
            }
            else if (valueType == typeof(int))
            {
                _valueField = new IntegerField
                {
                    style =
                    {
                        flexGrow = 1
                    }
                } as BaseField<TValue>;
            }
            else if (valueType == typeof(bool))
            {
                _valueField = new Toggle
                {
                    style =
                    {
                        marginBottom = 0,
                        marginTop = 0
                    }
                } as BaseField<TValue>;
            }
            else
            {
                throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                    nameof(TValue));
            }

            // Param popup field
            _paramPopup = new PopupField<ParamInfo>
            {
                formatSelectedValueCallback = FormatSelectedValue,
                formatListItemCallback = FormatListItem
            };
            _paramPopup.RegisterValueChangedCallback(OnLinkedParamChanged);

            if (IsLinkedToParam)
            {
                _paramContainer.Add(_paramPopup);
            }
            else
            {
                _paramContainer.Add(_valueField);
            }
        }

        public void SetParamChoices(List<ParamInfo> choices)
        {
            _paramPopup.choices = choices;
        }

        public void SetParamInfo(ParamInfo paramInfo)
        {
            Assert.IsNotNull(paramInfo);

            IsLinkedToParam = !paramInfo.IsLiteral;
            if (IsLinkedToParam)
            {
                _linkedParam = paramInfo;
                _paramPopup.value = _linkedParam;
                _paramPopup.MarkDirtyRepaint();
            }
            else
            {
                var valueType = typeof(TValue);
                if (valueType == typeof(float))
                {
                    _valueField.value = (TValue)(object)paramInfo.GetRawValue();
                }
                else if (valueType == typeof(int))
                {
                    _valueField.value = (TValue)(object)Mathf.RoundToInt(paramInfo.GetRawValue());
                }
                else if (valueType == typeof(bool))
                {
                    _valueField.value = (TValue)(object)(!Mathf.Approximately(0, paramInfo.GetRawValue()));
                }
                else
                {
                    throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                        nameof(valueType));
                }

                IsLinkedToParam = false;
            }

            RefreshParamView();
        }

        public bool GetParamInfo(out ParamInfo paramInfo)
        {
            if (IsLinkedToParam)
            {
                if (_linkedParam == null)
                {
                    paramInfo = ParamInfo.CreateLiteral();
                    return false;
                }

                paramInfo = _linkedParam;
                return true;
            }

            var valueType = typeof(TValue);
            if (valueType == typeof(float))
            {
                paramInfo = ParamInfo.CreateLiteral(ParamType.Float, (float)(object)_valueField.value);
                return true;
            }

            if (valueType == typeof(int))
            {
                paramInfo = ParamInfo.CreateLiteral(ParamType.Int, (int)(object)_valueField.value);
                return true;
            }

            if (valueType == typeof(bool))
            {
                var rawValue = (bool)(object)_valueField.value ? 1 : 0;
                paramInfo = ParamInfo.CreateLiteral(ParamType.Bool, rawValue);
                return true;
            }

            throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                nameof(valueType));
        }


        private string FormatListItem(ParamInfo paramInfo)
        {
            return paramInfo == null ? string.Empty : $"{paramInfo.Name} ({paramInfo.Type})";
        }

        private string FormatSelectedValue(ParamInfo paramInfo)
        {
            return paramInfo == null ? string.Empty : $"{paramInfo.Name} ({paramInfo.Type})";
        }

        private void OnClickParamLinkImage(MouseDownEvent _)
        {
            IsLinkedToParam = !IsLinkedToParam;
            RefreshParamView();
        }

        private void OnLinkedParamChanged(ChangeEvent<ParamInfo> evt)
        {
            _linkedParam = _paramPopup.value;
        }

        private void RefreshParamView()
        {
            _paramLinkImage.image = GetParamLinkIcon(IsLinkedToParam);

            if (IsLinkedToParam)
            {
                if (_paramContainer.Contains(_valueField))
                {
                    _paramContainer.Remove(_valueField);
                }

                _paramContainer.Add(_paramPopup);
            }
            else
            {
                if (_paramContainer.Contains(_paramPopup))
                {
                    _paramContainer.Remove(_paramPopup);
                }

                _paramContainer.Add(_valueField);
            }
        }


        #region Param link icon

        private const string _paramLinkedIconName = "Linked";
        private const string _paramLinkedIconName_Dark = "d_Linked";
        private const string _paramUnLinkedIconName = "UnLinked";
        private const string _paramUnLinkedIconName_Dark = "d_UnLinked";


        private Texture2D GetParamLinkIcon(bool isLinked)
        {
            if (isLinked)
            {
                var iconName = EditorGUIUtility.isProSkin
                    ? _paramLinkedIconName_Dark
                    : _paramLinkedIconName;
                return EditorGUIUtility.Load(iconName) as Texture2D;
            }
            else
            {
                var iconName = EditorGUIUtility.isProSkin
                    ? _paramUnLinkedIconName_Dark
                    : _paramUnLinkedIconName;
                return EditorGUIUtility.Load(iconName) as Texture2D;
            }
        }

        #endregion
    }
}
