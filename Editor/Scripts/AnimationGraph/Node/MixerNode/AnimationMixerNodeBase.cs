using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class AnimationMixerNodeBase : AnimationGraphNode
    {
        // shield Node.inputContainer
        [Obsolete("Please use InputPairContainer or InputContainerRoot.", true)]
        public new VisualElement inputContainer => null;

        protected VisualElement InputContainerRoot => base.inputContainer;

        protected VisualElement InputPairContainer { get; }


        private readonly List<PlayableInput> _inputs = new List<PlayableInput>();

        private Port _outputPort;


        protected AnimationMixerNodeBase(NodeData nodeData, bool isRebuild)
            : base(nodeData)
        {
            InputPairContainer = CreateInputPairContainer();
            CreateAddPortButton();

            if (!isRebuild)
            {
                AddOutputPort(nodeData.IsRootNode, null);
                AddInputPort(null, null);

                RefreshExpandedState();
                RefreshPorts();
            }
        }


        private VisualElement CreateInputPairContainer()
        {
            var inputPairContainer = new VisualElement
            {
                name = "input-pair-container",
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    height = new Length(100, LengthUnit.Percent)
                }
            };
            InputContainerRoot.Add(inputPairContainer);

            return inputPairContainer;
        }

        private void CreateAddPortButton()
        {
            if (!Application.isPlaying)
            {
                var addPortButton = new Button(() => { AddInputPort(null, null); })
                {
                    name = "add-port-button",
                    text = "Add Port"
                };
                InputContainerRoot.Add(addPortButton);
            }
        }


        public override void RebuildPorts()
        {
            _inputs.Clear();
            InputPairContainer.Clear();

            NodeData.SortPorts();
            for (var i = 0; i < NodeData.Ports.Count; i++)
            {
                var portData = NodeData.Ports[i];
                if (portData.Direction == Direction.Output)
                {
                    AddOutputPort(NodeData.IsRootNode, portData.Guid);
                    continue;
                }

                if ((portData.Index != NodeData.Ports[i + 1].Index) ||
                    !TryFindInputPortData(portData, NodeData.Ports[i + 1],
                        out var playablePortData, out var weightPortData))
                {
                    throw new DataMisalignedException();
                }

                AddInputPort(playablePortData.Guid, weightPortData.Guid);
                i++;
            }

            RefreshExpandedState();
            RefreshPorts();
        }


        protected void AddInputPort(string playablePortGuid, string weightPortGuid)
        {
            Assert.IsTrue(
                    (string.IsNullOrEmpty(playablePortGuid) && string.IsNullOrEmpty(weightPortGuid)) ||
                    (!string.IsNullOrEmpty(playablePortGuid) && !string.IsNullOrEmpty(weightPortGuid))
            );

            var isRebuild = !string.IsNullOrEmpty(playablePortGuid);
            var playablePort = InstantiatePort(Direction.Input, typeof(Playable), playablePortGuid);
            var weightPort = InstantiatePort(Direction.Input, typeof(float), weightPortGuid);
            var input = new PlayableInput(playablePort, weightPort, _inputs.Count, DeleteInputPort);
            _inputs.Add(input);
            InputPairContainer.Add(input);

            var playablePortData = (PortData)input.PlayablePort.userData;
            Ports.Add(playablePortData.Guid, playablePort);
            var weightPortData = (PortData)input.WeightPort.userData;
            Ports.Add(weightPortData.Guid, weightPort);

            if (!isRebuild)
            {
                NodeData.Ports.Add(playablePortData);
                NodeData.Ports.Add(weightPortData);
            }

            RefreshPortsData();
        }

        protected void AddOutputPort(bool isRootNode, string portGuid)
        {
            var isRebuild = !string.IsNullOrEmpty(portGuid);
            _outputPort = InstantiatePort(Direction.Output, typeof(Playable), portGuid);
            _outputPort.portName = "Output";
            _outputPort.portColor = Colors.AnimationPlayableColor;
            _outputPort.SetEnabled(!isRootNode);
            outputContainer.Add(_outputPort);

            var portData = (PortData)_outputPort.userData;
            Ports.Add(portData.Guid, _outputPort);

            if (!isRebuild)
            {
                NodeData.Ports.Add(portData);
            }
        }


        private bool TryFindInputPortData(PortData a, PortData b,
            out PortData playablePortData, out PortData weightPortData)
        {
            playablePortData = default;
            weightPortData = default;

            var aType = Type.GetType(a.TypeAssemblyQualifiedName);
            if (aType == typeof(Playable))
            {
                playablePortData = a;
            }
            else if (aType == typeof(float))
            {
                weightPortData = a;
            }
            else
            {
                return false;
            }

            var bType = Type.GetType(b.TypeAssemblyQualifiedName);
            if (bType == typeof(Playable))
            {
                playablePortData = b;
            }
            else if (bType == typeof(float))
            {
                weightPortData = b;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void DeleteInputPort(PlayableInput target)
        {
            // todo edge still remains in graph
            target.DisconnectAll();

            _inputs.Remove(target);
            InputPairContainer.Remove(target);

            NodeData.Ports.Remove((PortData)target.PlayablePort.userData);
            NodeData.Ports.Remove((PortData)target.WeightPort.userData);

            RefreshPortsData();
        }

        private void RefreshPortsData()
        {
            for (int i = 0; i < _inputs.Count; i++)
            {
                var input = _inputs[i];
                input.SetIndex(i);
                input.Deletable = _inputs.Count > 1;
                ((PortData)input.PlayablePort.userData).Index = i;
                ((PortData)input.WeightPort.userData).Index = i;
            }
        }
    }
}