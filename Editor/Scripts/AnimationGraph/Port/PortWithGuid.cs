using System;
using UnityEditor.Experimental.GraphView;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public class PortWithGuid : Port
    {
        public static PortWithGuid CreateInput(Type type)
        {
            return new PortWithGuid(Orientation.Horizontal, Direction.Input, Capacity.Single, type);
        }

        public static PortWithGuid CreateOutput(Type type)
        {
            return new PortWithGuid(Orientation.Horizontal, Direction.Output, Capacity.Single, type);
        }


        public string Guid { get; }


        public PortWithGuid(PortData portData)
            : base(Orientation.Horizontal, portData.Direction, Capacity.Single,
                Type.GetType(portData.TypeAssemblyQualifiedName))
        {
            Guid = portData.Guid;
        }

        public PortWithGuid(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    }
}