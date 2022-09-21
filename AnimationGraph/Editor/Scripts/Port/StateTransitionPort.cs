using System;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.Port
{
    public class StateTransitionPort : GraphPort
    {
        protected StateTransitionPort(Direction portDirection, Type portType)
            : base(portDirection, portType)
        {
        }


        public new static StateTransitionPort Create<TEdge>(Direction direction, Type portType)
            where TEdge : UEdge, new()
        {
            var connectorListener = new EdgeConnectorListener();
            var port = new StateTransitionPort(direction, portType)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
    }

    public class StateTransitionPortPair : VisualElement
    {
        public bool IsLayoutDirty { get; set; }

        public FlexDirection FlexDirection { get; private set; }

        public StateNode OwnerNode { get; }

        public StateTransitionPortPair ConnectedPortPair
        {
            get
            {
                if (!_inputPort.connected && !_outputPort.connected) return null;

                if (_outputPort.connected)
                {
                    return (StateTransitionPortPair)_outputPort.connections.First().input.parent;
                }

                if (_inputPort.connected)
                {
                    return (StateTransitionPortPair)_inputPort.connections.First().output.parent;
                }

                return null;
            }
        }

        public StateNode ConnectedNode
        {
            get => ConnectedPortPair?.OwnerNode;
        }


        private const float _PAIR_CONTAINER_SIZE = 40;

        private readonly StateTransitionPort _inputPort;

        private readonly StateTransitionPort _outputPort;


        public StateTransitionPortPair(StateNode ownerNode)
        {
            OwnerNode = ownerNode;

            style.width = _PAIR_CONTAINER_SIZE;
            style.height = _PAIR_CONTAINER_SIZE;
            style.justifyContent = Justify.SpaceBetween;

            _inputPort = StateTransitionPort.Create<StateTransitionEdge>(Direction.Input,
                typeof(Playable));
            _outputPort = StateTransitionPort.Create<StateTransitionEdge>(Direction.Output,
                typeof(Playable));
        }

        public void UpdateView()
        {
            if (!IsLayoutDirty) return;
            if (!_inputPort.connected && !_outputPort.connected) return;

            // Update self
            InternalUpdateView(this, 0);

            // Update connected pair
            var connectedPortPair = ConnectedPortPair;
            if (connectedPortPair != null && connectedPortPair.IsLayoutDirty)
            {
                InternalUpdateView(connectedPortPair, 1);
            }
        }


        private static void InternalUpdateView(StateTransitionPortPair pair, int outputPortIndex)
        {
            // Calculate elements direction
            Vector2 start, end;
            if (pair._outputPort.connected)
            {
                start = pair._outputPort.OwnerNode.localBound.center;
                end = ((FlowingGraphEdge)pair._outputPort.connections.First()).InputPort.OwnerNode.localBound
                    .center;
            }
            else
            {
                start = pair._inputPort.OwnerNode.localBound.center;
                end = ((FlowingGraphEdge)pair._inputPort.connections.First()).OutputPort.OwnerNode.localBound
                    .center;
            }

            // Set elements direction
            var slope = (start.y - end.y) / (start.x - end.x);
            var closerToHorizon = (slope >= -1 && slope <= 1);
            pair.FlexDirection = closerToHorizon ? FlexDirection.Column : FlexDirection.Row;
            pair.style.flexDirection = pair.FlexDirection;

            // Set elements order
            var inputPortIndex = outputPortIndex == 0 ? 1 : 0;
            if (pair[outputPortIndex] != pair._outputPort)
            {
                pair.PlaceBehind(pair[inputPortIndex]);
            }

            // Set ports orientation
            var orientationProp = typeof(UPort).GetProperty(nameof(UPort.orientation));
            object boxedOrientation = closerToHorizon ? Orientation.Horizontal : Orientation.Vertical;
            orientationProp!.SetValue(pair._inputPort, boxedOrientation);
            orientationProp.SetValue(pair._outputPort, boxedOrientation);

            foreach (var connection in pair._inputPort.connections)
            {
                connection.OnPortChanged(true);
            }

            foreach (var connection in pair._outputPort.connections)
            {
                connection.OnPortChanged(false);
            }

            pair.IsLayoutDirty = false;
        }
    }
}
