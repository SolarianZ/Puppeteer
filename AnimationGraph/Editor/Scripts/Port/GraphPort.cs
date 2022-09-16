using System;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Port
{
    public class GraphPort : UPort
    {
        public GraphNode OwnerNode => (GraphNode)node;

        public GraphNode ConnectedNode
        {
            get
            {
                if (!connected) return null;

                return (GraphNode)(direction == Direction.Input
                    ? connections.First().output.node
                    : connections.First().input.node);
            }
        }

        public event Action<GraphPort> OnConnected;

        public event Action<GraphPort> OnDisconnected;


        protected GraphPort(Direction portDirection, Type portType, Capacity portCapacity = Capacity.Single)
            : base(Orientation.Horizontal, portDirection, portCapacity, portType)
        {
            this.AddOnConnectListener(OnPortConnected);
            this.AddOnDisconnectListener(OnPortDisconnected);
        }


        protected virtual void OnPortConnected(UPort otherPort)
        {
            OnConnected?.Invoke(otherPort as GraphPort);
        }

        protected virtual void OnPortDisconnected(UPort otherPort)
        {
            OnDisconnected?.Invoke(otherPort as GraphPort);
        }


        public static GraphPort Create<TEdge>(Direction direction, Type portType,
            Capacity capacity = Capacity.Single) where TEdge : Edge, new()
        {
            var connectorListener = new EdgeConnectorListener();
            var port = new GraphPort(direction, portType, capacity)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }
}
