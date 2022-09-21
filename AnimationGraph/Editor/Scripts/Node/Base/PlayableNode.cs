using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract partial class PlayableNode : GraphNode
    {
        public const string INPUT_PORT_NAME_PREFIX = "Input";

        public override string Guid => NodeData.Guid;

        internal Playable Output { get; set; }

        internal List<GraphPort> InputPorts { get; } = new List<GraphPort>();

        internal GraphPort OutputPort { get; }

        internal PlayableNodeData NodeData { get; }


        protected PlayableNode(AnimationGraphAsset graphAsset, PlayableNodeData nodeData) : base(graphAsset)
        {
            NodeData = nodeData;

            OutputPort = InstantiatePort(Direction.Output, typeof(Playable));
            OutputPort.portColor = ColorTool.GetColor(typeof(Playable));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);

            SetPosition(new Rect(NodeData.EditorPosition, Vector2.zero));
        }


        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }


        protected void AddInputPortElement(int index)
        {
            var inputPort = InstantiatePort(Direction.Input, typeof(Playable));
            inputPort.portColor = ColorTool.GetColor(typeof(Playable));

            base.inputContainer.Insert(index, inputPort);
            InputPorts.Insert(index, inputPort);

            UpdatePortName(index, InputPorts.Count - 1);
            RefreshPorts();
            RaiseNodeChangedEvent();
        }

        protected void RemoveInputPortElement(int index)
        {
            var inputPort = InputPorts[index];
            if (inputPort.connected)
            {
                var connections = inputPort.connections.ToArray();
                inputPort.DisconnectAll();
                GraphView.DeleteElements(connections);
            }

            base.inputContainer.RemoveAt(index);
            InputPorts.RemoveAt(index);

            UpdatePortName(index, InputPorts.Count - 1);
            RaiseNodeChangedEvent();
        }

        protected void ReorderInputPortElement(int fromIndex, int toIndex)
        {
            (InputPorts[fromIndex], InputPorts[toIndex]) = (InputPorts[toIndex], InputPorts[fromIndex]);
            var targetPort = base.inputContainer[fromIndex];
            base.inputContainer.RemoveAt(fromIndex);
            base.inputContainer.Insert(toIndex, targetPort);

            UpdatePortName(fromIndex, toIndex);
            RaiseNodeChangedEvent();
        }

        protected void UpdatePortName(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                var inputPort = InputPorts[i];
                inputPort.portName = $"{INPUT_PORT_NAME_PREFIX} {i.ToString()}";
            }
        }
    }

    // API Masks
    public partial class PlayableNode
    {
        // ReSharper disable once InconsistentNaming
        [Obsolete("Use AddInputPort() or RemoveInputPort or ReorderInputPort() instead.", true)]
        protected new VisualElement inputContainer => base.inputContainer;
    }
}
