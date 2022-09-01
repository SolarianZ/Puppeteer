using System.Linq;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public sealed class RootNode : AnimationGraphNode
    {
        public AnimationGraphPort InputPort { get; }

        public PlayableNode InputNode => InputPort.connected
            ? (PlayableNode)InputPort.connections.First().output.node
            : null;


        public RootNode() : base("RootNode")
        {
            title = "Graph Output";

            InputPort = AnimationGraphPort.Create<AnimationGraphEdge>(Direction.Input, typeof(Playable));
            InputPort.portName = "Input Pose";
            InputPort.portColor = ColorTool.GetColor(typeof(Playable));
            inputContainer.Add(InputPort);

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // No option
        }
    }
}
