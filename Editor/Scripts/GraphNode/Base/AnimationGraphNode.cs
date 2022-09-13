﻿using System;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphPort;
using UnityEditor.Experimental.GraphView;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public abstract class AnimationGraphNode : GraphViewNode
    {
        public string Guid { get; }

        protected virtual string NodeName
        {
            get => title;
            set => title = value;
        }


        protected AnimationGraphNode(string guid)
        {
            Guid = guid;
        }


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