using System;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEntryNode : StateNode
    {
        public const string NODE_GUID = "StateMachineEntryNode";

        public override string Guid => NODE_GUID;

        public override string StateName
        {
            get => nameof(StateMachineEntryNode);
            internal set => throw new InvalidOperationException();
        }

        public string DestStateNodeGuid
        {
            get => _graphData.RootNodeGuid;
            internal set => _graphData.RootNodeGuid = value;
        }

        private readonly GraphData.GraphData _graphData;


        public StateMachineEntryNode(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, 
                new StateNodeData(NODE_GUID),
                new GraphData.GraphData(NODE_GUID, NODE_GUID, GraphType.StateMachine))
        {
            _graphData = graphData;

            title = "State Machine Entry";
            titleContainer.style.backgroundColor = ColorTool.GetColor<StateMachineEntryNode>();

            // Capabilities
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new StateMachineEntryNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }

        public override StateTransitionEdge AddTransition(StateNode destNode, out bool dataDirty)
        {
            // Only allow one transition
            if (OutputTransitions.Count > 0)
            {
                if (OutputTransitions[0].IsConnection(this, destNode))
                {
                    dataDirty = false;
                    return OutputTransitions[0];
                }

                var edgesToRemove = ViewOnlyDisconnectAll();
                GraphView.DeleteElements(edgesToRemove);
            }

            var edge = ViewOnlyConnect(destNode);
            edge.IsEntryEdge = true;

            // Transition data
            _graphData.RootNodeGuid = destNode.Guid;
            dataDirty = true;

            return edge;
        }
    }
}
