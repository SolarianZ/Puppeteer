using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationScriptEditorNode : MixerGraphEditorNode
    {
        internal new AnimationScriptNode Node => (AnimationScriptNode)base.Node;

        private AnimationScriptNodeInspector _inspector;


        public AnimationScriptEditorNode(AnimationGraphAsset graphAsset, AnimationScriptNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Animation Script";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphEditorNode> GetInspector()
        {
            _inspector ??= new AnimationScriptNodeInspector(GraphAsset.Parameters,
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
                Node.MixerInputs[portIndex].InputNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;
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
                Node.MixerInputs[portIndex].InputNodeGuid = null;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortDisconnected(edge);
        }


        private void RestoreInputPortElement()
        {
            // Add one input for new created node
            if (ExtraInfo.IsCreateFromContextualMenu)
            {
                Node.MixerInputs.Add(new MixerInputData());
            }

            for (var i = 0; i < Node.MixerInputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
