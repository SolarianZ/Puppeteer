using System;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Port
{
    public class GraphPort : UPort
    {
        public GraphEditorNode OwnerNode => (GraphEditorNode)node;

        // public GraphNode ConnectedNode
        // {
        //     get
        //     {
        //         if (!connected) return null;
        //
        //         return (GraphNode)(direction == Direction.Input
        //             ? connections.First().output.node
        //             : connections.First().input.node);
        //     }
        // }

        public event Action<UEdge> OnConnected;

        public event Action<UEdge> OnDisconnected;


        protected GraphPort(Direction portDirection, Type portType)
            : base(Orientation.Horizontal, portDirection, Capacity.Single, portType)
        {
        }


        public override void Connect(UEdge edge)
        {
            base.Connect(edge);
            OnConnected?.Invoke(edge);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            OnDisconnected?.Invoke(edge);
        }


        public static GraphPort Create<TEdge>(Direction direction, Type portType)
            where TEdge : UEdge, new()
        {
            var connectorListener = new EdgeConnectorListener();
            var port = new GraphPort(direction, portType)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }
}
