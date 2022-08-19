using System;
using UnityEditor.Experimental.GraphView;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [Serializable]
    public class PortData
    {
        public string Guid;

        public int Index;

        public Direction Direction;

        public string TypeAssemblyQualifiedName;
    }
}