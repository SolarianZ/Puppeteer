using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [System.Obsolete]
    public class AnimationOutputNode : AnimationGraphNode
    {
        private Port _inputPort;


        public AnimationOutputNode() : base(true)
        {
            title = "Animation Output";

            CreatePorts();
            RefreshExpandedState();
            RefreshPorts();
        }

        private void CreatePorts()
        {
            _inputPort = InstantiatePort(Direction.Input, typeof(Playable));
            _inputPort.portName = "Source";
            _inputPort.portColor = Colors.AnimationPlayableColor;
            inputContainer.Add(_inputPort);
        }
    }
}