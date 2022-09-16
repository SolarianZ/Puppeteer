using GBG.AnimationGraph.Editor.Port;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    // TODO: StateTransitionEdge
    public class StateTransitionEdge : UEdge
    {
        public GraphPort InputPort => input as GraphPort;

        public GraphPort OutputPort => output as GraphPort;
    }
}
