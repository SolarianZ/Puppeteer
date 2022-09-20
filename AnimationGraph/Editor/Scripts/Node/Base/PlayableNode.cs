using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract class PlayableNode : GraphNode
    {
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
    }
}
