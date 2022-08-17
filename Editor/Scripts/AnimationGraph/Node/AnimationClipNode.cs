using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class AnimationClipNode : AnimationGraphNode
    {
        private Port _inputPort;

        private Port _outputPort;


        public AnimationClipNode() : base(false)
        {
            title = "Animation Clip";

            CreatePorts();
            RefreshExpandedState();
            RefreshPorts();
        }

        private void CreatePorts()
        {
            _inputPort = InstantiatePort(Direction.Input, typeof(AnimationClip));
            _inputPort.portName = "Clip";
            _inputPort.portColor = Colors.AnimationPlayableColor;
            inputContainer.Add(_inputPort);

            _outputPort = InstantiatePort(Direction.Output, typeof(Playable));
            _outputPort.portName = "Output";
            _outputPort.portColor = Colors.AnimationPlayableColor;
            outputContainer.Add(_outputPort);
        }
    }
}
