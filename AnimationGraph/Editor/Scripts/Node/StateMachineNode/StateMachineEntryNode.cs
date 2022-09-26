using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEntryNode : StateNode
    {
        public const string NODE_GUID = "StateMachineEntryNode";

        public override string Guid => NODE_GUID;

        public string DestStateNodeGuid => _graphData.RootNodeGuid;

        private readonly GraphData.GraphData _graphData;


        public StateMachineEntryNode(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, new StateNodeData(NODE_GUID))
        {
            _graphData = graphData;

            title = "State Machine Entry";
            titleContainer.style.backgroundColor = new Color(30 / 255f, 110 / 255f, 55 / 255f);

            // Capabilities
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            RefreshPorts();
            RefreshExpandedState();
        }

        // TODO: Only allow one transition!

        public override StateTransitionEdge AddTransition(StateNode destNode)
        {
            if (OutputTransitions.Count > 0)
            {
                if (OutputTransitions[0].IsConnection(this, destNode))
                {
                    return OutputTransitions[0];
                }

                GraphView.RemoveElement(OutputTransitions[0]);
                RemoveTransition(OutputTransitions[0]);
            }

            var edge = ViewOnlyConnect(destNode);
            edge.IsEntryEdge = true;

            // Transition data
            _graphData.RootNodeGuid = destNode.Guid;

            return edge;
        }
    }
}
