using System;
using System.Linq;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphNode;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphPort
{
    public class AnimationGraphPort : GraphViewPort
    {
        public AnimationGraphNode OwnerNode => node as AnimationGraphNode;

        public AnimationGraphNode ConnectedNode
        {
            get
            {
                if (!connected) return null;

                return (AnimationGraphNode)(direction == Direction.Input
                    ? connections.First().output.node
                    : connections.First().input.node);
            }
        }


        protected AnimationGraphPort(Direction portDirection, Type portType,
            Capacity portCapacity = Capacity.Single)
            : base(Orientation.Horizontal, portDirection, portCapacity, portType)
        {
        }


        public static AnimationGraphPort Create(Direction direction, Type portType,
            Capacity capacity = Capacity.Single)
        {
            return Create<AnimationGraphEdge>(direction, portType, capacity);
        }

        public static AnimationGraphPort Create<TEdge>(Direction direction, Type portType,
            Capacity capacity = Capacity.Single) where TEdge : Edge, new()
        {
            var connectorListener = new EdgeConnectorListener();
            var port = new AnimationGraphPort(direction, portType, capacity)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }
}
