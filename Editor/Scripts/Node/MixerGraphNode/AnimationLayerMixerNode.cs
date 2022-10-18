using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationLayerMixerNode : MixerGraphNode
    {
        internal new AnimationLayerMixerNodeData NodeData => (AnimationLayerMixerNodeData)base.NodeData;

        private AnimationLayerMixerNodeInspector _inspector;


        public AnimationLayerMixerNode(AnimationGraphAsset graphAsset, AnimationLayerMixerNodeData nodeData,
            NodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "Layer Mixer";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }


        public override IInspector<GraphNode> GetInspector()
        {
            _inspector ??= new AnimationLayerMixerNodeInspector(GraphAsset.Parameters,
                AddInputPortElement, RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTarget(this);

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
            // Add two inputs for new created node
            if (ExtraInfo.IsCreateFromContextualMenu)
            {
                NodeData.MixerInputs.Add(new LayerMixerInputData
                {
                    InputWeightParam = new ParamGuidOrValue(null, 1), // Base layer
                });
                NodeData.MixerInputs.Add(new LayerMixerInputData());
            }

            for (var i = 0; i < NodeData.MixerInputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
