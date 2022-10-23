﻿using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using UnityEngine;
using UnityEngine.Assertions;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateEditorNode : StateGraphEditorNode
    {
        public override string Guid => Node.Guid;

        internal override List<Transition> Transitions => Node.Transitions;

        public string MixerGraphGuid => Node.MixerGraphGuid;

        internal StateNode Node { get; }

        public override string StateName
        {
            get => Graph.Name;
            internal set
            {
                Graph.Name = value;
                title = Graph.Name;
            }
        }


        public StateEditorNode(AnimationGraphAsset graphAsset, StateNode node,
            Graph.Graph graph) : base(graphAsset, graph)
        {
            Assert.AreEqual(node.MixerGraphGuid, graph.Guid);

            Node = node;

            SetPosition(new Rect(Node.EditorPosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.EditorPosition = newPos.position;
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new StateNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
