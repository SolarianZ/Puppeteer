using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class InputPair : VisualElement
    {
        public int Index { get; private set; } = -1;

        public bool Deletable { get; set; } = true;

        public Port PlayablePort { get; }

        public Port WeightPort { get; }


        private Button _deleteButton;

        private readonly Action<InputPair> _onDelete;


        public InputPair(AnimationGraphNode owner, int index,
            Color playablePortColor, Type playablePortType, Action<InputPair> onDelete)
        {
            #region Ports

            PlayablePort = owner.InstantiatePort(Orientation.Horizontal,
                Direction.Input, Port.Capacity.Single, playablePortType);
            PlayablePort.portName = $"Input {index}";
            PlayablePort.portColor = playablePortColor;

            WeightPort = owner.InstantiatePort(Orientation.Horizontal,
                Direction.Input, Port.Capacity.Single, typeof(float));
            WeightPort.portName = $"{PlayablePort.portName} weight";

            Add(PlayablePort);
            Add(WeightPort);

            #endregion

            #region Delete Button

            _onDelete = onDelete;
            RegisterCallback<MouseEnterEvent>(OnMouseOver);
            RegisterCallback<MouseLeaveEvent>(OnMouseOut);

            #endregion
        }

        public void SetIndex(int index)
        {
            Index = index;
            PlayablePort.portName = $"Input {index}";
            WeightPort.portName = $"{PlayablePort.portName} weight";
        }


        private void OnMouseOver(MouseEnterEvent evt)
        {
            if (!Deletable)
            {
                return;
            }

            _deleteButton = new Button(() => { _onDelete(this); })
            {
                text = "Delete",
                style =
                {
                    position = Position.Absolute,
                    left = 0,
                    right = 0,
                    top = 0,
                    bottom = 0
                }
            };

            Add(_deleteButton);
        }

        private void OnMouseOut(MouseLeaveEvent evt)
        {
            if (_deleteButton != null)
            {
                Remove(_deleteButton);
                _deleteButton = null;
            }
        }
    }
}