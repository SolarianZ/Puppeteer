using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationMixerNode : AnimationGraphNode
    {
        public override bool AllowMultiInput => true;


        private readonly List<InputPair> _inputPairs = new List<InputPair>();

        private VisualElement _inputPairContainer;

        private Port _outputPort;

        private Button _addPortButton;


        public AnimationMixerNode() : this(false)
        {
        }

        public AnimationMixerNode(bool isRootNode) : base(isRootNode)
        {
            title = isRootNode ? "[Root]\nAnimation Mixer" : "Animation Mixer";

            _inputPairContainer = new VisualElement
            {
                name = "input-pair-container",
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    height = new Length(100, LengthUnit.Percent)
                }
            };
            inputContainer.Add(_inputPairContainer);

            var addPortButton = new Button(AddInputPort)
            {
                name = "add-port-button",
                text = "Add Port"
            };
            inputContainer.Add(addPortButton);

            CreatePorts();
            RefreshExpandedState();
            RefreshPorts();
        }

        private void CreatePorts()
        {
            AddInputPort();

            _outputPort = InstantiatePort(Direction.Output, typeof(Playable));
            _outputPort.portName = "Output";
            _outputPort.portColor = Colors.AnimationPlayableColor;
            _outputPort.SetEnabled(!IsRootNode);
            outputContainer.Add(_outputPort);
        }

        private void AddInputPort()
        {
            var inputPair = new InputPair(this, _inputPairs.Count, Colors.AnimationPlayableColor,
                typeof(Playable), DeleteInputPort);
            _inputPairs.Add(inputPair);
            _inputPairContainer.Add(inputPair);

            RefreshPortDeletableState();
        }

        private void DeleteInputPort(InputPair target)
        {
            _inputPairs.Remove(target);
            _inputPairContainer.Remove(target);

            // todo delete edges

            RefreshPortDeletableState();
        }

        private void RefreshPortDeletableState()
        {
            for (int i = 0; i < _inputPairs.Count; i++)
            {
                _inputPairs[i].SetIndex(i);
                _inputPairs[i].Deletable = _inputPairs.Count > 1;
            }
        }
    }
}