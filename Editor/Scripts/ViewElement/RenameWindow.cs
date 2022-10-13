using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.ViewElement
{
    public class RenameWindow : EditorWindow
    {
        public static void Open(string oldName, IEnumerable<string> conflictingNames, Action<string, string> onComplete)
        {
            var window = Resources.FindObjectsOfTypeAll<RenameWindow>().FirstOrDefault();
            if (!window)
            {
                window = GetWindow<RenameWindow>(true, "Rename", true);
            }

            window._oldName = oldName;
            window._conflictingNames = conflictingNames;
            window._onComplete = onComplete;
            window._oldNameField.SetValueWithoutNotify(oldName);
            window._newNameField.SetValueWithoutNotify(oldName);
        }


        private TextField _oldNameField;

        private TextField _newNameField;

        private Label _messageLabel;

        private Button _applyButton;

        private string _oldName;

        private string _newName;

        private IEnumerable<string> _conflictingNames;

        private Action<string, string> _onComplete;

        private Color _normalMessageColor;

        private static readonly Color _invalidMessageColor = Color.red;


        private void OnEnable()
        {
            var body = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 10,
                    paddingBottom = 10,
                }
            };
            rootVisualElement.Add(body);

            // Old name
            _oldNameField = new TextField("Old Name");
            _oldNameField.SetEnabled(false);
            body.Add(_oldNameField);

            // User input new name
            _newNameField = new TextField("New Name");
            _newNameField.RegisterValueChangedCallback(OnNameChanged);
            body.Add(_newNameField);

            // Message label
            _messageLabel = new Label
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    marginTop = 8,
                }
            };
            _messageLabel.RegisterCallback<GeometryChangedEvent>(OnMessageLabelGeometryChanged);
            body.Add(_messageLabel);

            // Apply button
            _applyButton = new Button(ApplyRename)
            {
                text = "Apply",
                style = { marginBottom = 8, }
            };
            rootVisualElement.Add(_applyButton);
        }

        private void OnDisable()
        {
            _conflictingNames = null;
            _onComplete = null;
        }

        private void OnLostFocus()
        {
            Close();
        }


        private void OnMessageLabelGeometryChanged(GeometryChangedEvent evt)
        {
            _messageLabel.UnregisterCallback<GeometryChangedEvent>(OnMessageLabelGeometryChanged);
            _normalMessageColor = _messageLabel.resolvedStyle.color;

            OnNameChanged(_oldName);
        }

        private void ApplyRename()
        {
            _onComplete?.Invoke(_oldName, _newName);
            Close();
        }

        private void OnNameChanged(ChangeEvent<string> evt)
        {
            OnNameChanged(evt.newValue);
        }

        private void OnNameChanged(string userInputName)
        {
            _applyButton.SetEnabled(false);
            _messageLabel.style.color = _invalidMessageColor;

            // Check empty
            if (string.IsNullOrEmpty(userInputName))
            {
                _messageLabel.text = "Name can't be empty.";
                return;
            }

            if (userInputName.Equals(_oldName))
            {
                _messageLabel.text = "Name has not changed.";
                return;
            }

            // Check conflicting names
            if (_conflictingNames != null && _conflictingNames.Contains(userInputName))
            {
                _messageLabel.text = "New name conflicted with other names.";
                return;
            }

            // Check characters
            for (int i = 0; i < userInputName.Length; i++)
            {
                var c = userInputName[i];
                if (c >= 'a' && c <= 'z') continue;
                if (c >= 'A' && c <= 'Z') continue;
                if (c == '_') continue;
                if (i != 0 && (c >= '0' && c <= '9')) continue;

                _messageLabel.text = "Name can consist of letters(A-Z,a-z), digits(0-9), underscores(_), " +
                                     "and the first character must not be a digit.";
                return;
            }

            _newName = userInputName;
            _messageLabel.style.color = _normalMessageColor;
            _applyButton.SetEnabled(true);
        }
    }
}
