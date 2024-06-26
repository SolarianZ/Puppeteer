﻿using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationLayerMixerEditorNode : MixerGraphEditorNode
    {
        internal new AnimationLayerMixerNode Node => (AnimationLayerMixerNode)base.Node;

        private AnimationLayerMixerNodeInspector _inspector;


        public AnimationLayerMixerEditorNode(AnimationGraphAsset graphAsset, AnimationLayerMixerNode node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset, node, extraInfo)
        {
            title = "Layer Mixer";

            RestoreInputPortElement();

            RefreshPorts();
            RefreshExpandedState();
        }


        public override IInspector<GraphEditorNode> GetInspector()
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
                Node.MixerInputs[portIndex].InputNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;
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
                Node.MixerInputs[portIndex].InputNodeGuid = null;
                _inspector?.RefreshMixerInputList();
            }

            base.OnPortDisconnected(edge);
        }


        private void RestoreInputPortElement()
        {
            // Add two inputs for new created node
            if (ExtraInfo.IsCreateFromContextualMenu)
            {
                Node.MixerInputs.Add(new LayeredNodeInput
                {
                    InputWeightParam = new ParamGuidOrValue(null, 1), // Base layer
                });
                Node.MixerInputs.Add(new LayeredNodeInput());
            }

            for (var i = 0; i < Node.MixerInputs.Count; i++)
            {
                AddInputPortElement(i);
            }
        }
    }
}
