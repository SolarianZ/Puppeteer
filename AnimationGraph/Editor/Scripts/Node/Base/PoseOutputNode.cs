using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class PoseOutputNode : GraphNode
    {
        public override string Guid => "PoseOutputNode";

        public string InputPlayableNodeGuid { get; private set; }


        public PoseOutputNode(AnimationGraphAsset graphAsset) : base(graphAsset)
        {
            title = "Pose Output";

            var inputPort = InstantiatePort(Direction.Input, typeof(Playable));
            inputPort.portName = "Input Pose";
            inputPort.portColor = ColorTool.GetColor(typeof(Playable));
            inputPort.OnConnected += OnPortConnected;
            inputPort.OnDisconnected += OnPortDisconnected;
            inputContainer.Add(inputPort);

            RefreshPorts();
            RefreshExpandedState();
        }


        private void OnPortConnected(GraphPort otherPort)
        {
            InputPlayableNodeGuid = otherPort.OwnerNode.Guid;
        }

        private void OnPortDisconnected(GraphPort otherPort)
        {
            InputPlayableNodeGuid = null;
        }
    }
}
