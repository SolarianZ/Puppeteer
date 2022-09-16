using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.ViewElement
{
    public class RenameWindow : EditorWindow
    {
        public static void Open(string oldName, Action<string> onComplete)
        {
            var window = FindObjectOfType<RenameWindow>();
            if (!window)
            {
                window = GetWindow<RenameWindow>(true, "Rename", true);
            }

            window._oldName = oldName;
            window._onComplete = onComplete;
            window._oldNameField.SetValueWithoutNotify(oldName);
            window._newNameField.SetValueWithoutNotify(oldName);
            window._resultNameField.SetValueWithoutNotify(oldName);
        }


        private TextField _oldNameField;

        private TextField _newNameField;

        private TextField _resultNameField;

        private string _oldName;

        private Action<string> _onComplete;


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

            // Result name
            _resultNameField = new TextField("Result Name");
            _resultNameField.SetEnabled(false);
            body.Add(_resultNameField);

            // Close button
            var closeButton = new Button(ApplyRename)
            {
                text = "Apply",
            };
            rootVisualElement.Add(closeButton);
        }

        private void OnLostFocus()
        {
            Close();
        }


        private void ApplyRename()
        {
            _onComplete?.Invoke(_resultNameField.value);
            Close();
        }

        private void OnNameChanged(ChangeEvent<string> evt)
        {
            var newName = evt.newValue;
            if (_oldName == newName)
            {
                return;
            }

            // Check name
            for (int i = 0; i < newName.Length; i++)
            {
                var c = newName[i];
                if (c >= 'a' && c <= 'z') continue;
                if (c >= 'A' && c <= 'Z') continue;
                if (c == '_') continue;
                if (i != 0 && (c >= '0' && c <= '9')) continue;

                newName = newName.Remove(i, 1);
                i--;
            }

            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            _resultNameField.SetValueWithoutNotify(newName);
        }
    }
}
