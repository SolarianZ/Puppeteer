using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using UnityEngine;
using UnityEngine.Assertions;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class StateEditorNode : StateGraphEditorNode
    {
        public override string Guid => NodeData.Guid;

        internal override List<Transition> Transitions => NodeData.Transitions;

        public string MixerGraphGuid => NodeData.MixerGraphGuid;

        internal StateNodeData NodeData { get; }

        public override string StateName
        {
            get => GraphData.Name;
            internal set
            {
                GraphData.Name = value;
                title = GraphData.Name;
            }
        }


        public StateEditorNode(AnimationGraphAsset graphAsset, StateNodeData nodeData,
            GraphData.GraphData graphData) : base(graphAsset, graphData)
        {
            Assert.AreEqual(nodeData.MixerGraphGuid, graphData.Guid);

            NodeData = nodeData;

            SetPosition(new Rect(NodeData.EditorPosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            var inspector = new StateNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
