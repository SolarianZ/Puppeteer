using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class ScriptEditorNode : MixerGraphEditorNode
    {
        internal new ScriptNode Node => (ScriptNode)base.Node;

        private ScriptNodeInspector _inspector;


        public ScriptEditorNode(AnimationGraphAsset graphAsset, ScriptNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Animation Script";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new ScriptNodeInspector(GraphAsset.Parameters,
                AddInputPortElement, RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTarget(this);

            return _inspector;
        }


        protected override void OnPortConnected(Edge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                Node.Inputs[portIndex].InputNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortConnected(edge);
        }

        protected override void OnPortDisconnected(Edge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                Node.Inputs[portIndex].InputNodeGuid = null;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortDisconnected(edge);
        }


        private void RestoreInputPortElement()
        {
            // Add one input for new created node
            if (ExtraInfo.IsCreateFromContextualMenu)
            {
                Node.Inputs.Add(new WeightedNodeInput());
            }

            for (var i = 0; i < Node.Inputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
