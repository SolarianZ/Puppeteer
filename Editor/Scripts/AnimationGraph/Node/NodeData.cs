using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [Serializable]
    public class NodeData
    {
        public string Title;

        public string Guid;

        public bool IsRootNode;

        public string TypeAssemblyQualifiedName;

        public Vector2 Position;

        public List<PortData> Ports = new List<PortData>();


        public void SortPorts()
        {
            Ports.Sort((a, b) =>
            {
                if (a.Direction == Direction.Output) return -1;
                if (b.Direction == Direction.Output) return -1;
                if (a.Index < b.Index) return -1;
                if (a.Index > b.Index) return 1;
                return 0;
            });
        }
    }

    [Serializable]
    public class PortData
    {
        public string Guid;

        public int Index;

        public Direction Direction;

        public string TypeAssemblyQualifiedName;
    }

    [Serializable]
    public class EdgeData
    {
        public string FromNodeGuid;

        public string FromPortGuid;

        public string ToNodeGuid;

        public string ToPortGuid;
    }
}