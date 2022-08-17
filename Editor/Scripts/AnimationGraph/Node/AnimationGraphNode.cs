using System;
using UnityEditor.Experimental.GraphView;
using UNode = UnityEditor.Experimental.GraphView.Node;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class AnimationGraphNode : UNode
    {
        public bool IsRootNode { get; }

        public Guid Guid { get; }

        public virtual bool AllowMultiInput => false;


        protected AnimationGraphNode(bool isRootNode)
        {
            this.IsRootNode = isRootNode;

            Guid = Guid.NewGuid();
        }

        public AnimationGraphNode(Guid guid)
        {
            this.Guid = guid;
        }


        protected Port InstantiatePort(Direction direction, Type type)
        {
            return InstantiatePort(Orientation.Horizontal,
                direction, Port.Capacity.Single, type);
        }
    }
}