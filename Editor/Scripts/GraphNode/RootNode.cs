using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class RootNode : AnimationGraphNode
    {
        public PlayableNode Input { get; private set; }


        public RootNode()
        {
            title = "Graph Output";

            var inputPort = AnimationGraphPort.Create<AnimationGraphEdge>(Direction.Input, typeof(Playable));
            inputPort.portName = "Input Pose";
            inputPort.portColor = ColorTool.GetColor(typeof(Playable));
            inputContainer.Add(inputPort);

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void OnInputConnected(AnimationGraphEdge edge)
        {
            Input = (PlayableNode)edge.Output.Node;
        }

        public override void OnInputDisconnected(AnimationGraphEdge edge)
        {
            Input = null;
        }
    }
}
