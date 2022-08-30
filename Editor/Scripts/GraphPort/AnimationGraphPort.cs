using System;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphNode;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphPort
{
    public class AnimationGraphPort : GraphViewPort
    {
        public AnimationGraphNode Node => node as AnimationGraphNode;


        protected AnimationGraphPort(Direction portDirection, Type portType,
            Capacity portCapacity = Capacity.Single)
            : base(Orientation.Horizontal, portDirection, portCapacity, portType)
        {
        }


        public static AnimationGraphPort Create<TEdge>(Direction direction, Type type,
            Capacity capacity = Capacity.Single) where TEdge : Edge, new()
        {
            var connectorListener = new EdgeConnectorListener();
            var port = new AnimationGraphPort(direction, type, capacity)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }
}
