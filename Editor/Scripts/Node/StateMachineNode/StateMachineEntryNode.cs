using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEntryNode : StateGraphNode
    {
        public const string NODE_GUID = "StateMachineEntryNode";

        public override string Guid => NODE_GUID;

        internal override List<Transition> Transitions => _nodeData.Transitions;
        
        public override string StateName
        {
            get => nameof(StateMachineEntryNode);
            internal set => throw new InvalidOperationException();
        }

        public string DestStateNodeGuid
        {
            get => GraphData.RootNodeGuid;
            internal set => GraphData.RootNodeGuid = value;
        }


        private readonly StateNodeData _nodeData;
        

        public StateMachineEntryNode(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, graphData)
        {
            _nodeData = new StateNodeData(NODE_GUID);
            
            title = "State Machine Entry";
            titleContainer.style.backgroundColor = ColorTool.GetColor<StateMachineEntryNode>();

            // Capabilities
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            SetPosition(new Rect(GraphData.EditorGraphRootNodePosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            GraphData.EditorGraphRootNodePosition = newPos.position;
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new StateMachineEntryNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }

        public override StateTransitionEdge AddTransition(StateGraphNode destNode, out bool dataDirty)
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
            GraphData.RootNodeGuid = destNode.Guid;
            dataDirty = true;

            return edge;
        }
    }
}
