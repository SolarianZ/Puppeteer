using System;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphPort;
using UnityEditor.Experimental.GraphView;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public abstract class AnimationGraphNode : GraphViewNode
    {
        public abstract void OnInputConnected(AnimationGraphEdge edge);

        public abstract void OnInputDisconnected(AnimationGraphEdge edge);

        public override Port InstantiatePort(Orientation orientation,
            Direction direction, Port.Capacity capacity, Type type)
        {
            return AnimationGraphPort.Create<AnimationGraphEdge>(direction, type, capacity);
        }


        protected AnimationGraphPort InstantiatePort(Direction direction, Type type,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return (AnimationGraphPort)InstantiatePort(Orientation.Horizontal, direction, capacity, type);
        }
    }
}
