using GBG.AnimationGraph.NodeData;
using UnityEditor;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract class StateNode : GraphNode
    {
        public override string Guid => NodeData.Guid;

        protected NodeDataBase NodeData { get; }


        protected StateNode(AnimationGraphAsset graphAsset, NodeDataBase nodeData) : base(graphAsset)
        {
            NodeData = nodeData;
        }


        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.EditorPosition = newPos.position;
        }
    }
}
