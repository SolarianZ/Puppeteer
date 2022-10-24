using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateMachineEntryEditorNode : StateGraphEditorNode
    {
        public const string NODE_GUID = "StateMachineEntryNode";

        public override string Guid => NODE_GUID;

        internal override List<Transition> Transitions => _node.Transitions;
        
        public override string StateName
        {
            get => nameof(StateMachineEntryEditorNode);
            internal set => throw new InvalidOperationException();
        }

        public string DestStateNodeGuid
        {
            get => GraphLayer.RootNodeGuid;
            internal set => GraphLayer.RootNodeGuid = value;
        }


        private readonly StateNode _node;
        

        public StateMachineEntryEditorNode(AnimationGraphAsset graphAsset, Graph.GraphLayer graphLayer)
            : base(graphAsset, graphLayer)
        {
            _node = new StateNode(NODE_GUID);
            
            title = "State Machine Entry";
            titleContainer.style.backgroundColor = ColorTool.GetColor<StateMachineEntryEditorNode>();

            // Capabilities
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            SetPosition(new Rect(GraphLayer.EditorGraphRootNodePosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            GraphLayer.EditorGraphRootNodePosition = newPos.position;
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new StateMachineEntryNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }

        public override StateTransitionEdge AddTransition(StateGraphEditorNode destNode, out bool dataDirty)
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
            GraphLayer.RootNodeGuid = destNode.Guid;
            dataDirty = true;

            return edge;
        }
    }
}
