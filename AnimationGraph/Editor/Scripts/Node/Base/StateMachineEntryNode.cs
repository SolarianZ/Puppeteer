using GBG.AnimationGraph.Editor.Port;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEntryNode : GraphNode
    {
        public override string Guid => "StateMachineEntryNode";

        public string DestStateNodeGuid { get; private set; }


        public StateMachineEntryNode(AnimationGraphAsset graphAsset) : base(graphAsset)
        {
            title = "State Machine Entry";

            var outputPort = InstantiatePort(Direction.Output, typeof(bool));
            outputPort.portName = "Dest State";
            // outputPort.portColor = ColorTool.GetColor(typeof(bool));
            outputPort.OnConnected += OnPortConnected;
            outputPort.OnDisconnected += OnPortDisconnected;
            outputContainer.Add(outputPort);

            RefreshPorts();
            RefreshExpandedState();
        }


        private void OnPortConnected(GraphPort otherPort)
        {
            DestStateNodeGuid = otherPort.OwnerNode.Guid;
        }

        private void OnPortDisconnected(GraphPort otherPort)
        {
            DestStateNodeGuid = null;
        }
    }
}
