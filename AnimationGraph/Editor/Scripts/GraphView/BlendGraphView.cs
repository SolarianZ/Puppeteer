using GBG.AnimationGraph.Editor.Node;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class BlendGraphView : GraphViewBase
    {
        public override GraphNode RootNode => PoseOutputNode;

        public PoseOutputNode PoseOutputNode { get; }


        public BlendGraphView(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, graphData)
        {
            PoseOutputNode = new PoseOutputNode(GraphAsset);
            AddElement(PoseOutputNode);

            // TODO: Correct root node position
            // Nodes may moved and we don't save root node's position,
            // so here we need to set root node's position to right of its input node
        }
    }
}
