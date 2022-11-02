using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract partial class MixerGraphEditorNode : GraphEditorNode
    {
        public const string INPUT_PORT_NAME_PREFIX = "Input";

        public override string Guid => Node.Guid;

        internal Playable Output { get; set; }

        internal List<GraphPort> InputPorts { get; } = new List<GraphPort>();

        internal GraphPort OutputPort { get; }

        internal NodeBase Node { get; }

        protected EditorNodeExtraInfo ExtraInfo { get; }


        protected MixerGraphEditorNode(AnimationGraphAsset graphAsset, NodeBase node,
            EditorNodeExtraInfo extraInfo) : base(graphAsset)
        {
            Node = node;
            ExtraInfo = extraInfo;

            OutputPort = InstantiatePort(Direction.Output, typeof(Playable));
            OutputPort.portColor = ColorTool.GetColor(typeof(Playable));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);

            SetPosition(new Rect(Node.EditorPosition, Vector2.zero));
        }


        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.EditorPosition = newPos.position;
        }


        protected void AddInputPortElement(int index)
        {
            var inputPort = InstantiatePort(Direction.Input, typeof(Playable));
            inputPort.portColor = ColorTool.GetColor(typeof(Playable));

            InputPorts.Insert(index, inputPort);
            base.inputContainer.Insert(index, inputPort);

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

            InputPorts.RemoveAt(index);
            base.inputContainer.RemoveAt(index);

            UpdatePortName(index, InputPorts.Count - 1);
            RaiseNodeChangedEvent();
        }

        protected void ReorderInputPortElement(int fromIndex, int toIndex)
        {
            var targetPort = InputPorts[fromIndex];
            InputPorts.RemoveAt(fromIndex);
            InputPorts.Insert(toIndex, targetPort);

            var targetPortElem = base.inputContainer[fromIndex];
            base.inputContainer.RemoveAt(fromIndex);
            base.inputContainer.Insert(toIndex, targetPortElem);

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
    public partial class MixerGraphEditorNode
    {
        // ReSharper disable once InconsistentNaming
        [Obsolete("Use AddInputPort() or RemoveInputPort or ReorderInputPort() instead.", true)]
        protected new VisualElement inputContainer => base.inputContainer;
    }
}
