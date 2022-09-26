﻿using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;

namespace GBG.AnimationGraph.Editor.Node
{
    public sealed class AnimationClipNode : PlayableNode
    {
        public AnimationClipNode(AnimationGraphAsset graphAsset, AnimationClipNodeData nodeData)
            : base(graphAsset, nodeData)
        {
            title = "Animation Clip";

            RefreshPorts();
            RefreshExpandedState();
        }

        public override IInspector<GraphNode> GetInspector()
        {
            var inspector = new AnimationClipNodeInspector();
            inspector.SetTarget(this);

            return inspector;
        }
    }
}
