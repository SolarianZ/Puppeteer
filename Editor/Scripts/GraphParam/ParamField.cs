using System;
using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphParam
{
    public abstract class ParamField : VisualElement
    {
        public abstract string ParamName { get; }

        public event Action<ParamField> OnValueChanged;


        public abstract void SetLabel(string label);

        public abstract void SetParamChoices(List<ParamInfo> choices);

        public abstract void SetParamInfo(ParamInfo paramInfo);

        public abstract bool GetParamInfo(out ParamInfo paramInfo);


        protected void RaiseBaseValueChangedEvent()
        {
            OnValueChanged?.Invoke(this);
        }
    }

    public class ParamField<TValue> : ParamField
        where TValue : struct
    {
        public override string ParamName => Linked ? LinkedParam.Name : string.Empty;

        protected BaseField<TValue> ValueField { get; }

        protected ParamInfo LinkedParam { get; private set; }

        protected bool Linked { get; private set; }


        public new event Action<ParamField<TValue>> OnValueChanged;


        private readonly Label _label;

        private readonly Image _paramLinkImage;

        private readonly VisualElement _paramContainer;

        private readonly PopupField<ParamInfo> _paramPopup;


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
                image = GetParamLinkIcon(Linked),
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
                ValueField = new FloatField
                {
                    style =
                    {
                        flexGrow = 1
                    }
                } as BaseField<TValue>;
            }
            else if (valueType == typeof(int))
            {
                ValueField = new IntegerField
                {
                    style =
                    {
                        flexGrow = 1
                    }
                } as BaseField<TValue>;
            }
            else if (valueType == typeof(bool))
            {
                ValueField = new Toggle
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

            ValueField.RegisterValueChangedCallback(OnParamValueChanged);

            // Param popup field
            _paramPopup = new PopupField<ParamInfo>
            {
                formatSelectedValueCallback = FormatParamInfo,
                formatListItemCallback = FormatParamInfo,
                style =
                {
                    flexGrow = 1,
                }
            };
            _paramPopup.RegisterValueChangedCallback(OnLinkedParamChanged);

            if (Linked)
            {
                _paramContainer.Add(_paramPopup);
            }
            else
            {
                _paramContainer.Add(ValueField);
            }
        }

        public override void SetLabel(string label)
        {
            _label.text = label;
        }

        public override void SetParamChoices(List<ParamInfo> choices)
        {
            _paramPopup.choices = choices;
        }

        public override void SetParamInfo(ParamInfo paramInfo)
        {
            Assert.IsNotNull(paramInfo);

            Linked = !paramInfo.IsLiteral;
            if (Linked)
            {
                if (LinkedParam != paramInfo)
                {
                    if (LinkedParam != null)
                    {
                        LinkedParam.EditorOnNameChanged -= OnLinkedParamNameChanged;
                    }

                    LinkedParam = paramInfo;
                    LinkedParam.EditorOnNameChanged += OnLinkedParamNameChanged;
                    _paramPopup.value = LinkedParam;
                    _paramPopup.MarkDirtyRepaint();
                }
            }
            else
            {
                var valueType = typeof(TValue);
                if (valueType == typeof(float))
                {
                    ValueField.value = (TValue)(object)paramInfo.GetRawValue();
                }
                else if (valueType == typeof(int))
                {
                    ValueField.value = (TValue)(object)Mathf.RoundToInt(paramInfo.GetRawValue());
                }
                else if (valueType == typeof(bool))
                {
                    ValueField.value = (TValue)(object)(!Mathf.Approximately(0, paramInfo.GetRawValue()));
                }
                else
                {
                    throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                        nameof(valueType));
                }
            }

            RefreshParamView();
        }

        public override bool GetParamInfo(out ParamInfo paramInfo)
        {
            if (Linked && LinkedParam != null)
            {
                paramInfo = LinkedParam;
                return true;
            }

            object boxedValue = Linked ? 0f : ValueField.value;
            var valueType = typeof(TValue);
            if (valueType == typeof(float))
            {
                paramInfo = ParamInfo.CreateLiteral(ParamType.Float, (float)boxedValue);
                return true;
            }

            if (valueType == typeof(int))
            {
                paramInfo = ParamInfo.CreateLiteral(ParamType.Int, (int)boxedValue);
                return true;
            }

            if (valueType == typeof(bool))
            {
                var rawValue = (bool)boxedValue ? 1 : 0;
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
            RaiseBaseValueChangedEvent();
        }

        private void OnClickParamLinkImage(MouseDownEvent _)
        {
            Linked = !Linked;
            RefreshParamView();

            OnValueChanged?.Invoke(this);
            RaiseBaseValueChangedEvent();
        }

        private void OnLinkedParamChanged(ChangeEvent<ParamInfo> evt)
        {
            LinkedParam = _paramPopup.value;

            OnValueChanged?.Invoke(this);
            RaiseBaseValueChangedEvent();
        }

        private void OnLinkedParamNameChanged(ParamInfo paramInfo)
        {
            _paramPopup.SetValueWithoutNotify(paramInfo);
        }

        private void RefreshParamView()
        {
            _paramLinkImage.image = GetParamLinkIcon(Linked);

            if (Linked)
            {
                if (_paramContainer.Contains(ValueField))
                {
                    _paramContainer.Remove(ValueField);
                }

                _paramContainer.Add(_paramPopup);
            }
            else
            {
                if (_paramContainer.Contains(_paramPopup))
                {
                    _paramContainer.Remove(_paramPopup);
                }

                _paramContainer.Add(ValueField);
            }
        }


        #region Param link icon

        private static Texture2D GetParamLinkIcon(bool isLinked)
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
