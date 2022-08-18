using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class AnimationGraphNode : UNode
    {
        public NodeData NodeData { get; }

        protected Dictionary<string, Port> Ports { get; } = new Dictionary<string, Port>();


        protected AnimationGraphNode(NodeData nodeData)
        {
            NodeData = nodeData;
            SetPosition(new Rect(nodeData.Position, Vector2.zero));
        }


        public bool TryFindPort(string guid, out Port port)
        {
            return Ports.TryGetValue(guid, out port);
        }

        public abstract void RebuildPorts();

        public Port InstantiatePort(Direction direction, Type type, string guid = null)
        {
            var port = base.InstantiatePort(Orientation.Horizontal,
                direction, Port.Capacity.Single, type);
            port.userData = new PortData
            {
                Guid = guid ?? NewGuid(),
                Direction = direction,
                TypeAssemblyQualifiedName = type.AssemblyQualifiedName
            };

            return port;
        }

        [Obsolete("Please use InstantiatePort method with other signature.", true)]
        public new Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            throw new InvalidOperationException();
        }


        protected static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}