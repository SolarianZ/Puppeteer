using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class PoseOutputNode : GraphNode
    {
        public override string Guid => "PoseOutputNode";

        public GraphPort PoseInputPort { get; }

        public string RootPlayableNodeGuid => _graphData.RootNodeGuid;

        private readonly GraphData.GraphData _graphData;


        public PoseOutputNode(AnimationGraphAsset graphAsset, GraphData.GraphData graphData) : base(graphAsset)
        {
            _graphData = graphData;

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

            SetPosition(new Rect(_graphData.EditorGraphRootNodePosition, Vector2.zero));

            RefreshPorts();
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _graphData.EditorGraphRootNodePosition = newPos.position;
        }

        protected override void OnPortConnected(UEdge edge)
        {
            var graphEdge = (FlowingGraphEdge)edge;
            _graphData.RootNodeGuid = graphEdge.OutputPort.OwnerNode.Guid;

            base.OnPortConnected(edge);
        }

        protected override void OnPortDisconnected(UEdge edge)
        {
            _graphData.RootNodeGuid = null;

            base.OnPortDisconnected(edge);
        }
    }
}
