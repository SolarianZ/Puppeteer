using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class PlayableInput : VisualElement
    {
        public int Index { get; private set; }

        public bool Deletable { get; set; }

        public Port PlayablePort { get; }

        public Port WeightPort { get; }


        private Button _deleteButton;

        private readonly Action<PlayableInput> _onDelete;


        public PlayableInput(Port playablePort, Port weightPort,
            int index, Action<PlayableInput> onDelete)
        {
            #region Ports

            // playable
            PlayablePort = playablePort;
            PlayablePort.portName = $"Input {index}";
            PlayablePort.portColor = Colors.AnimationPlayableColor;
            Add(PlayablePort);

            // weight
            WeightPort = weightPort;
            WeightPort.portName = $"{PlayablePort.portName} weight";
            Add(WeightPort);

            #endregion

            #region Delete Button

            _onDelete = onDelete;
            RegisterCallback<MouseEnterEvent>(OnMouseOver);
            RegisterCallback<MouseLeaveEvent>(OnMouseOut);

            #endregion
        }

        public void DisconnectAll()
        {
            PlayablePort.DisconnectAll();
            WeightPort.DisconnectAll();
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