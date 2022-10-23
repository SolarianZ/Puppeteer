using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    // TODO: Sync inputs
    public sealed class AnimationMixerEditorNode : MixerGraphEditorNode
    {
        internal new AnimationMixerNodeData NodeData => (AnimationMixerNodeData)base.NodeData;

        private AnimationMixerNodeInspector _inspector;


        public AnimationMixerEditorNode(AnimationGraphAsset graphAsset, AnimationMixerNodeData nodeData,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, nodeData, extraInfo)
        {
            title = "Mixer";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }


        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new AnimationMixerNodeInspector(GraphAsset.Parameters,
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
                NodeData.MixerInputs.Add(new MixerInputData());
                NodeData.MixerInputs.Add(new MixerInputData());
            }

            for (var i = 0; i < NodeData.MixerInputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
