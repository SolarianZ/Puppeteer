﻿using System;
using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphParam
{
    public class ParamField<TValue> : VisualElement
        where TValue : struct
    {
        public bool IsLinkedToParam { get; private set; }


        public event Action<ParamField<TValue>> OnValueChanged;


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
            style.paddingLeft = 4;
            style.paddingRight = 1;

            // Param link image
            _paramLinkImage = new Image
            {
                name = "param-link-icon",
                image = GetParamLinkIcon(IsLinkedToParam),
                style =
                {
                    width = 16,
                }
            };
            _paramLinkImage.RegisterCallback<MouseDownEvent>(OnClickParamLinkImage);
            Add(_paramLinkImage);

            // Label
            _label = new Label(label)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginLeft = 4,
                    marginRight = 4,
                    paddingLeft = 0,
                    paddingRight = 0,
                }
            };

            if (labelWidth != null)
            {
                _label.style.width = labelWidth.Value;
            }

            Add(_label);

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

            _valueField.RegisterValueChangedCallback(OnParamValueChanged);

            // Param popup field
            _paramPopup = new PopupField<ParamInfo>
            {
                formatSelectedValueCallback = FormatParamInfo,
                formatListItemCallback = FormatParamInfo
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

        public void SetLabel(string label)
        {
            _label.text = label;
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
                if (_linkedParam != paramInfo)
                {
                    if (_linkedParam != null)
                    {
                        _linkedParam.EditorOnNameChanged -= OnLinkedParamNameChanged;
                    }

                    _linkedParam = paramInfo;
                    _linkedParam.EditorOnNameChanged += OnLinkedParamNameChanged;
                    _paramPopup.value = _linkedParam;
                    _paramPopup.MarkDirtyRepaint();
                }
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


        private string FormatParamInfo(ParamInfo paramInfo)
        {
            return paramInfo == null ? string.Empty : $"{paramInfo.Name} ({paramInfo.Type})";
        }

        private void OnParamValueChanged(ChangeEvent<TValue> evt)
        {
            OnValueChanged?.Invoke(this);
        }

        private void OnClickParamLinkImage(MouseDownEvent _)
        {
            IsLinkedToParam = !IsLinkedToParam;
            RefreshParamView();

            OnValueChanged?.Invoke(this);
        }

        private void OnLinkedParamChanged(ChangeEvent<ParamInfo> evt)
        {
            _linkedParam = _paramPopup.value;

            OnValueChanged?.Invoke(this);
        }

        private void OnLinkedParamNameChanged(ParamInfo paramInfo)
        {
            _paramPopup.SetValueWithoutNotify(paramInfo);
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

        private Texture2D GetParamLinkIcon(bool isLinked)
        {
            const string PARAM_LINKED_ICON_NAME = "Linked";
            const string PARAM_LINKED_ICON_NAME_DARK = "d_Linked";
            const string PARAM_UN_LINKED_ICON_NAME = "UnLinked";
            const string PARAM_UNLINKED_ICON_NAME_DARK = "d_UnLinked";

            if (isLinked)
            {
                var iconName = EditorGUIUtility.isProSkin
                    ? PARAM_LINKED_ICON_NAME_DARK
                    : PARAM_LINKED_ICON_NAME;
                return EditorGUIUtility.Load(iconName) as Texture2D;
            }
            else
            {
                var iconName = EditorGUIUtility.isProSkin
                    ? PARAM_UNLINKED_ICON_NAME_DARK
                    : PARAM_UN_LINKED_ICON_NAME;
                return EditorGUIUtility.Load(iconName) as Texture2D;
            }
        }

        #endregion
    }
}
