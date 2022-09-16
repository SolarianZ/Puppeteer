using GBG.AnimationGraph.Editor.Node;

namespace GBG.AnimationGraph.Editor.GraphView
{
    public class StateMachineGraphView : GraphViewBase
    {
        public override GraphNode RootNode => StateMachineEntryNode;

        public StateMachineEntryNode StateMachineEntryNode { get; }


        public StateMachineGraphView(AnimationGraphAsset graphAsset, GraphData.GraphData graphData)
            : base(graphAsset, graphData)
        {
            StateMachineEntryNode = new StateMachineEntryNode(GraphAsset);
            AddElement(StateMachineEntryNode);
        }
    }
}
