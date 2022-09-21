using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationMixerNode : PlayableNode
    {
        private AnimationMixerNodeInspector _inspector;


        public AnimationMixerNode(AnimationGraphAsset graphAsset, AnimationMixerNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Mixer";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }


        public override GraphNodeInspector GetInspector()
        {
            _inspector ??= new AnimationMixerNodeInspector(GraphAsset.Parameters,
                AddInputPortElement, RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTargetNode(this);

            return _inspector;
        }


        protected override void OnPortConnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                NodeData.MixerInputs[portIndex].InputNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortConnected(edge);
        }

        protected override void OnPortDisconnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                NodeData.MixerInputs[portIndex].InputNodeGuid = null;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortDisconnected(edge);
        }


        private void RestoreInputPortElement()
        {
            for (var i = 0; i < NodeData.MixerInputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
