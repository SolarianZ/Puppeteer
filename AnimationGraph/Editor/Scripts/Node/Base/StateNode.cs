using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.NodeData;
using UnityEngine;
using UnityEngine.UIElements;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public class StateNode : GraphNode
    {
        private const float _CONNECTION_HANDLER_WIDTH = 8;


        public override string Guid => NodeData.Guid;

        // internal
        public List<StateTransitionPortPair> Transitions { get; } = new List<StateTransitionPortPair>();

        internal StateNodeData NodeData { get; }


        public StateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData) : base(graphAsset)
        {
            NodeData = nodeData;

            // Styles
            mainContainer.style.backgroundColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1.0f);
            mainContainer.style.justifyContent = Justify.Center;
            mainContainer.style.borderTopWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderBottomWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderLeftWidth = _CONNECTION_HANDLER_WIDTH;
            mainContainer.style.borderRightWidth = _CONNECTION_HANDLER_WIDTH;

            // Transitions
            // InputPort = (StateTransitionPort)InstantiatePort(Orientation.Horizontal, Direction.Input,
            //     UPort.Capacity.Multi, typeof(Playable));
            // InputPort.portName = null;
            // InputPort.portColor = ColorTool.GetColor(typeof(Playable));
            // InputPort.AddToClassList(STATE_INPUT_PORT_CLASS_NAME);
            // InputPort.StretchToParentSize();
            // mainContainer.Insert(0, InputPort);

            SetPosition(new Rect(NodeData.EditorPosition, Vector2.zero));
            RefreshPorts();
            RefreshExpandedState();

            // Events
            // mainContainer.AddManipulator(OutputPort.edgeConnector);
        }


        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }

        public StateTransitionPortPair GetOrCreatePortPair(StateNode destNode)
        {
            StateTransitionPortPair pair;
            if (destNode == null)
            {
                pair = Transitions.FirstOrDefault(p => p.ConnectedNode == null);
            }
            else
            {
                pair = Transitions.FirstOrDefault(p => p.ConnectedNode == destNode);
            }

            if (pair == null)
            {
                pair = new StateTransitionPortPair(this);
                Transitions.Add(pair);
            }

            return pair;
        }
    }
}
