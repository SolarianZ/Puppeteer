using GBG.AnimationGraph.Editor.Port;
using GBG.AnimationGraph.NodeData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract class PlayableNode : GraphNode
    {
        public override string Guid => NodeData.Guid;

        public abstract Playable Output { get; }

        public GraphPort OutputPort { get; }

        protected NodeDataBase NodeData { get; }


        protected PlayableNode(AnimationGraphAsset graphAsset, NodeDataBase nodeData) : base(graphAsset)
        {
            NodeData = nodeData;

            OutputPort = InstantiatePort(Direction.Output, typeof(Playable));
            outputContainer.Add(OutputPort);
        }


        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            NodeData.EditorPosition = newPos.position;
            EditorUtility.SetDirty(GraphAsset);

            // User may moves many nodes, so don't raise data changed event,
            // handle nodes move event in GraphView.graphViewChanged callback
            // RaiseNodeDataChangedEvent();
        }
    }
}
