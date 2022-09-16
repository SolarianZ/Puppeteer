using System;
using GBG.AnimationGraph.Editor.ViewElement;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Blackboard
{
    public class ParamField : VisualElement
    {
        // public const string PARAM_NAME_MATCH_REGEX = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public event Action<ParamInfo> OnParamChanged;

        public event Action<ParamInfo> OnWantDeleteParam;


        private readonly Label _typeLabel;

        private readonly Label _nameLabel;

        private readonly FloatField _floatField;

        private readonly IntegerField _intField;

        private readonly Toggle _boolField;

        private ParamInfo _paramInfo;


        public ParamField()
        {
            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;

            RegisterCallback<MouseDownEvent>(OnMouseClicked);

            // Type
            _typeLabel = new Label
            {
                style =
                {
                    width = 12,
                    fontSize = 12,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    marginRight = 10,
                }
            };
            Add(_typeLabel);

            // Name
            _nameLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(_nameLabel);

            // Float value
            _floatField = new FloatField
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            _floatField.RegisterValueChangedCallback(OnFloatValueChanged);

            // Int value
            _intField = new IntegerField
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            _intField.RegisterValueChangedCallback(OnIntValueChanged);

            // Bool value
            _boolField = new Toggle
            {
                style =
                {
                    width = Length.Percent(35),
                }
            };
            _boolField.RegisterValueChangedCallback(OnBoolValueChanged);
        }

        public void SetParamInfo(ParamInfo paramInfo)
        {
            if (_paramInfo == paramInfo) return;
            _paramInfo = paramInfo;

            // Type
            _typeLabel.text = _paramInfo.Type.ToString().Substring(0, 1);

            // Name
            _nameLabel.text = _paramInfo.Name;

            // Value
            if (Contains(_floatField)) Remove(_floatField);
            if (Contains(_intField)) Remove(_intField);
            if (Contains(_boolField)) Remove(_boolField);
            switch (_paramInfo.Type)
            {
                // Add value field and set value
                case ParamType.Float:
                    _floatField.SetValueWithoutNotify(_paramInfo.GetFloat());
                    Add(_floatField);
                    break;

                case ParamType.Int:
                    _intField.SetValueWithoutNotify(_paramInfo.GetInt());
                    Add(_intField);
                    break;

                case ParamType.Bool:
                    _boolField.SetValueWithoutNotify(_paramInfo.GetBool());
                    Add(_boolField);
                    break;

                default:
                    throw new ArgumentException();
            }
        }


        private void OnMouseClicked(MouseDownEvent evt)
        {
            // Left mouse button double click to rename param
            if (evt.button == 0 && evt.clickCount > 1)
            {
                RenameWindow.Open(_paramInfo.Name, OnNameChanged);
                return;
            }

            // Right mouse button click to show contextual menu
            if (evt.button == 1)
            {
                var menuPos = evt.mousePosition;
                var menu = new GenericDropdownMenu();

                // Rename
                menu.AddItem("Rename", false, () => { RenameWindow.Open(_paramInfo.Name, OnNameChanged); });

                // Delete
                menu.AddItem("Delete", false, () => { OnWantDeleteParam?.Invoke(_paramInfo); });

                menu.DropDown(new Rect(menuPos, Vector2.zero), this);
            }
        }


        private void OnNameChanged(string newName)
        {
            if (_nameLabel.text.Equals(newName))
            {
                return;
            }

            _nameLabel.text = newName;

            if (_paramInfo == null)
            {
                OnParamChanged?.Invoke(null);
                return;
            }

            if (_paramInfo.Name.Equals(newName))
            {
                return;
            }

            _paramInfo.EditorSetName(newName);

            OnParamChanged?.Invoke(_paramInfo);
        }

        private void OnFloatValueChanged(ChangeEvent<float> _)
        {
            _paramInfo?.SetFloat(_floatField.value);
            OnParamChanged?.Invoke(_paramInfo);
        }

        private void OnIntValueChanged(ChangeEvent<int> _)
        {
            _paramInfo?.SetInt(_intField.value);
            OnParamChanged?.Invoke(_paramInfo);
        }

        private void OnBoolValueChanged(ChangeEvent<bool> _)
        {
            _paramInfo?.SetBool(_boolField.value);
            OnParamChanged?.Invoke(_paramInfo);
        }
    }
}
