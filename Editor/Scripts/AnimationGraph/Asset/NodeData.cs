using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UObject = UnityEngine.Object;

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
    public class ValueNodeData : NodeData
    {
        public ValueSource Source;

        public string ParameterName;

        public float FloatValue;

        public int IntValue;

        public string StringValue;

        public UObject ObjectValue;
    }

    [Serializable]
    public class ObjectNodeData : NodeData
    {
        [SerializeReference]
        public UObject Object;
    }
}