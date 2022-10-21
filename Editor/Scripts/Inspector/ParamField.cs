using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Parameter;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public enum ParamLinkState
    {
        Unlinked = 0,

        Linked = 1,

        UnlinkedLocked = 2,

        LinkedLocked = 4,
    }

    public enum ParamActiveState
    {
        Active = 0,

        Inactive = 1,

        ActiveLocked = 2,

        InactiveLocked = 4,
    }

    public class ParamField : VisualElement
    {
        public const float MIN_ELEMENT_HEIGHT = 20;
        
        public event Action<ParamGuidOrValue> OnParamChanged;

        public event Action<bool> OnActivityChanged;


        private ParamGuidOrValue _serializedTarget;

        /// <summary>
        /// Parameters that the serialized target can bind to.
        /// </summary>
        private List<ParamInfo> _paramTable;

        private ParamType _paramType;

        private Vector2? _valueRange;

        private readonly Label _nameLabel;

        private readonly VisualElement _paramContainer;

        private BaseField<float> _floatField;

        private BaseField<int> _intField;

        private Toggle _boolField;

        /// <summary>
        /// Click this button to select bind target parameter.
        /// </summary>
        private Button _linkedParamButton;

        /// <summary>
        /// Used for indicate and toggle link state.
        /// </summary>
        private Image _linkIcon;

        private Toggle _activeToggle;

        private ParamLinkState _linkState;

        private ParamInfo _linkedParam;


        public ParamField(Length nameLabelWidth)
        {
            // Styles
            style.flexDirection = FlexDirection.Row;
            style.flexShrink = 0;
            style.marginLeft = 3;
            style.marginRight = 3;

            // Name label
            _nameLabel = new Label
            {
                style =
                {
                    width = nameLabelWidth,
                    marginRight = 2,
                    overflow = Overflow.Hidden,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(_nameLabel);

            // Value field
            _paramContainer = new VisualElement
            {
                name = "param-container",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                },
            };
            Add(_paramContainer);
        }

        public void SetParamTarget(string nameLabel, ParamGuidOrValue serializedTarget, ParamType targetType,
            List<ParamInfo> paramTable, ParamLinkState linkState, ParamActiveState activeState, Vector2? valueRange)
        {
            // Source data
            _serializedTarget = serializedTarget;
            _paramType = targetType;
            _paramTable = paramTable;
            _linkState = linkState;
            _valueRange = valueRange;
            _linkedParam = IsLinked()
                ? _paramTable.FirstOrDefault(p => p.Guid.Equals(_serializedTarget.Guid))
                : null;
            Assert.IsTrue(valueRange == null || valueRange.Value.x <= valueRange.Value.y);

            if (_linkedParam != null)
            {
                _linkedParam.EditorOnNameChanged += OnLinkedParamNameChanged;
            }

            // View data
            _nameLabel.text = nameLabel;
            _paramContainer.Clear();
            if (Contains(_linkIcon)) Remove(_linkIcon);
            if (Contains(_activeToggle)) Remove(_activeToggle);

            // Link to param
            if (IsLinked())
            {
                PrepareLinkedParamButton();
                _linkedParamButton.text =  GetLinkedParamButtonText();
                _paramContainer.Add(_linkedParamButton);
            }
            // Raw value
            else
            {
                _paramContainer.Add(PrepareValueField(true));
            }

            // Link icon
            switch (_linkState)
            {
                case ParamLinkState.Unlinked:
                case ParamLinkState.Linked:
                    if (_linkIcon == null)
                    {
                        _linkIcon = new Image
                        {
                            name = "param-link-icon",
                            image = GetParamLinkIcon(IsLinked()),
                            style =
                            {
                                width = 16,
                                marginLeft = 3,
                                marginRight = 3,
                            }
                        };
                        _linkIcon.RegisterCallback<MouseDownEvent>(OnParamLinkIconClicked);
                    }

                    Add(_linkIcon);
                    break;

                case ParamLinkState.UnlinkedLocked:
                case ParamLinkState.LinkedLocked:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_linkState), _linkState, null);
            }

            // Active toggle
            switch (activeState)
            {
                case ParamActiveState.Active:
                case ParamActiveState.Inactive:
                    if (_activeToggle == null)
                    {
                        _activeToggle = new Toggle
                        {
                            value = activeState == ParamActiveState.Active,
                            style =
                            {
                                marginLeft = 0,
                                marginRight = 0,
                                marginTop = 0,
                                marginBottom = 0,
                            }
                        };
                        _activeToggle.RegisterValueChangedCallback(OnActivityChangedInternal);
                    }

                    Add(_activeToggle);

                    _paramContainer.SetEnabled(activeState == ParamActiveState.Active);
                    _linkIcon?.SetEnabled(activeState == ParamActiveState.Active);
                    break;

                case ParamActiveState.ActiveLocked:
                    _paramContainer.SetEnabled(true);
                    _linkIcon?.SetEnabled(true);
                    break;

                case ParamActiveState.InactiveLocked:
                    _paramContainer.SetEnabled(false);
                    _linkIcon?.SetEnabled(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(activeState), activeState, null);
            }
        }


        private bool IsLinked()
        {
            return _linkState == ParamLinkState.Linked || _linkState == ParamLinkState.LinkedLocked;
        }

        private string GetLinkedParamButtonText()
        {
            return _linkedParam?.Name ?? "<Select a Parameter>";
        }

        private VisualElement PrepareValueField(bool forceCreateNew)
        {
            switch (_paramType)
            {
                case ParamType.Float:
                    var floatValue = _linkedParam?.GetFloat() ?? _serializedTarget.GetFloat();
                    if (_floatField == null || forceCreateNew)
                    {
                        _floatField = _valueRange != null
                            ? new Slider(_valueRange.Value.x, _valueRange.Value.y)
                            {
                                value = floatValue,
                                showInputField = true,
                            }
                            : _floatField = new FloatField
                            {
                                value = floatValue,
                            };
                        _floatField.style.flexGrow = 1;
                        _floatField.style.marginLeft = 0;
                        _floatField.style.marginRight = 0;
                        _floatField.RegisterValueChangedCallback(OnFloatValueChanged);
                    }

                    return _floatField;

                case ParamType.Int:
                    var intValue = _linkedParam?.GetInt() ?? _serializedTarget.GetInt();
                    if (_intField == null || forceCreateNew)
                    {
                        _intField = _valueRange != null
                            ? new SliderInt((int)_valueRange.Value.x, (int)_valueRange.Value.y)
                            {
                                value = intValue,
                                showInputField = true,
                            }
                            : _intField = new IntegerField
                            {
                                value = intValue,
                            };
                        _intField.style.flexGrow = 1;
                        _intField.style.marginLeft = 0;
                        _intField.style.marginRight = 0;
                        _intField.RegisterValueChangedCallback(OnIntValueChanged);
                    }

                    return _intField;

                case ParamType.Bool:
                    if (_boolField == null || forceCreateNew)
                    {
                        _boolField = new Toggle
                        {
                            value = _linkedParam?.GetBool() ?? _serializedTarget.GetBool(),
                        };
                        _boolField.style.flexGrow = 1;
                        _boolField.style.marginLeft = 0;
                        _boolField.style.marginRight = 0;
                        _boolField.RegisterValueChangedCallback(OnBoolValueChanged);
                    }

                    return _boolField;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_paramType), _paramType, null);
            }
        }

        /// <summary>
        /// Make sure linked param button exist.
        /// </summary>
        /// <returns></returns>
        private Button PrepareLinkedParamButton()
        {
            if (_linkedParamButton == null)
            {
                _linkedParamButton = new Button
                {
                    text = _linkedParam?.Name,
                    style =
                    {
                        flexGrow = 1,
                        marginLeft = 0,
                        marginRight = 0,
                    }
                };
                _linkedParamButton.clickable.clickedWithEventInfo += ShowParamPopupList;
            }

            return _linkedParamButton;
        }

        private void ShowParamPopupList(EventBase evt)
        {
            var menu = new GenericDropdownMenu();

            foreach (var paramInfo in _paramTable)
            {
                if (paramInfo.Type == _paramType)
                {
                    menu.AddItem(paramInfo.Name, paramInfo == _linkedParam, ChangeLinkedParam, paramInfo);
                }
                else
                {
                    menu.AddDisabledItem(paramInfo.Name, paramInfo == _linkedParam);
                }
            }

            menu.DropDown(new Rect(evt.originalMousePosition, Vector2.zero), _linkedParamButton);
        }


        private void OnFloatValueChanged(ChangeEvent<float> evt)
        {
            _serializedTarget.RawValue = evt.newValue;
            OnParamChanged?.Invoke(_serializedTarget);
        }

        private void OnIntValueChanged(ChangeEvent<int> evt)
        {
            _serializedTarget.RawValue = evt.newValue;
            OnParamChanged?.Invoke(_serializedTarget);
        }

        private void OnBoolValueChanged(ChangeEvent<bool> evt)
        {
            _serializedTarget.RawValue = evt.newValue ? 1 : 0;
            OnParamChanged?.Invoke(_serializedTarget);
        }

        private void OnLinkedParamNameChanged(ParamInfo _)
        {
            _linkedParamButton.text = _linkedParam.Name;
        }

        private void ChangeLinkedParam(object linkedParamObj)
        {
            if (_linkedParam != null)
            {
                _linkedParam.EditorOnNameChanged -= OnLinkedParamNameChanged;
                _linkedParamButton.text = null;
            }

            var linkedParam = (ParamInfo)linkedParamObj;
            _linkedParam = linkedParam;
            if (_linkedParam != null)
            {
                _linkedParam.EditorOnNameChanged += OnLinkedParamNameChanged;
                _linkedParamButton.text = _linkedParam.Name;
            }

            _serializedTarget.Guid = _linkedParam?.Guid;
            OnParamChanged?.Invoke(_serializedTarget);
        }

        private void OnParamLinkIconClicked(MouseDownEvent _)
        {
            switch (_linkState)
            {
                case ParamLinkState.Unlinked:
                    _linkState = ParamLinkState.Linked;
                    break;
                case ParamLinkState.Linked:
                    _linkState = ParamLinkState.Unlinked;
                    break;

                case ParamLinkState.UnlinkedLocked:
                case ParamLinkState.LinkedLocked:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var isLinked = IsLinked();
            _linkIcon.image = GetParamLinkIcon(isLinked);

            if (isLinked)
            {
                if (_linkedParam != null)
                {
                    _linkedParam.EditorOnNameChanged += OnLinkedParamNameChanged;
                }

                PrepareLinkedParamButton();
                _linkedParamButton.text =  GetLinkedParamButtonText();
                _paramContainer.Clear();
                _paramContainer.Add(_linkedParamButton);
            }
            else
            {
                if (_linkedParam != null)
                {
                    _linkedParam.EditorOnNameChanged -= OnLinkedParamNameChanged;
                }

                _paramContainer.Clear();
                _paramContainer.Add(PrepareValueField(false));
            }

            _serializedTarget.Guid = isLinked ? _linkedParam?.Guid : null;

            OnParamChanged?.Invoke(_serializedTarget);
        }

        private void OnActivityChangedInternal(ChangeEvent<bool> evt)
        {
            var isActive = evt.newValue;
            _paramContainer.SetEnabled(isActive);
            _linkIcon?.SetEnabled(isActive);

            OnActivityChanged?.Invoke(isActive);
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
