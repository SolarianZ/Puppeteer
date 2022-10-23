using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class BlendSpace2DEditorNode : MixerGraphEditorNode
    {
        internal new BlendSpace2DNodeData NodeData => (BlendSpace2DNodeData)base.NodeData;

        private BlendSpace2DNodeInspector _inspector;

        public BlendSpace2DEditorNode(AnimationGraphAsset graphAsset, BlendSpace2DNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "Blend Space 2D";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new BlendSpace2DNodeInspector(AddInputPortElement,
                RemoveInputPortElement, ReorderInputPortElement);
            _inspector.SetTarget(this);

            return _inspector;
        }


        protected override void OnPortConnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                NodeData.Samples[portIndex].InputNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;
                _inspector?.RefreshSampleInputList();
            }

            base.OnPortConnected(edge);
        }

        protected override void OnPortDisconnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            if (graphEdge.InputPort.OwnerNode == this)
            {
                var portIndex = InputPorts.IndexOf(graphEdge.InputPort);
                NodeData.Samples[portIndex].InputNodeGuid = null;
                _inspector?.RefreshSampleInputList();
            }

            base.OnPortDisconnected(edge);
        }


        private void RestoreInputPortElement()
        {
            for (var i = 0; i < NodeData.Samples.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
