using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Node;
using UnityEditor.Experimental.GraphView;

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

            // Callbacks
            graphViewChanged += OnGraphViewChanged;
        }

        public List<StateNode> GetCompatibleStateNodes(StateNode fromNode)
        {
            var nodeList = new List<StateNode>();
            foreach (var node in nodes)
            {
                if (node == fromNode || node is StateMachineEntryNode)
                {
                    continue;
                }

                nodeList.Add((StateNode)node);
            }

            return nodeList;
        }


        private new GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                if (element is StateNode stateNode)
                {
                    for (int i = 0; i < GraphData.Nodes.Count; i++)
                    {
                        if (GraphData.Nodes[i].Guid == stateNode.Guid)
                        {
                            GraphData.Nodes.RemoveAt(i);
                            break;
                        }
                    }
                }
            });

            RaiseGraphViewChangedEvent();

            return graphViewChange;
        }
    }
}
