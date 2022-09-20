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

            // Capabilities
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            // Destination state
            var outputPort = InstantiatePort(Direction.Output, typeof(bool));
            outputPort.portName = "Dest State";
            // outputPort.portColor = ColorTool.GetColor(typeof(bool));
            outputContainer.Add(outputPort);

            RefreshPorts();
            RefreshExpandedState();
        }

    }
}
