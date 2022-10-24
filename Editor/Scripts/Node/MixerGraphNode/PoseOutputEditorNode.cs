using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class PoseOutputEditorNode : GraphEditorNode
    {
        public override string Guid => "PoseOutputNode";

        public GraphPort PoseInputPort { get; }

        public string RootPlayableNodeGuid => _graphLayer.RootNodeGuid;

        private readonly Graph.GraphLayer _graphLayer;


        public PoseOutputEditorNode(AnimationGraphAsset graphAsset, Graph.GraphLayer graphLayer) : base(graphAsset)
        {
            _graphLayer = graphLayer;

            title = "Pose Output";

            // Capabilities
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;

            // Input port
            PoseInputPort = InstantiatePort(Direction.Input, typeof(Playable));
            PoseInputPort.portName = "Input Pose";
            PoseInputPort.portColor = ColorTool.GetColor(typeof(Playable));
            PoseInputPort.OnConnected += OnPortConnected;
            PoseInputPort.OnDisconnected += OnPortDisconnected;
            inputContainer.Add(PoseInputPort);

            SetPosition(new Rect(_graphLayer.EditorGraphRootNodePosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _graphLayer.EditorGraphRootNodePosition = newPos.position;
        }

        protected override void OnPortConnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            _graphLayer.RootNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;

            base.OnPortConnected(edge);
        }

        protected override void OnPortDisconnected(UEdge edge)
        {
            _graphLayer.RootNodeGuid = null;

            base.OnPortDisconnected(edge);
        }
    }
}
