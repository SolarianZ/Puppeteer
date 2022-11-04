using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    public class PrepareFrameArgs
    {
        public IReadOnlyList<NodeBase> InputNodes { get; }

        public IReadOnlyList<float> InputWeights { get; }

        public bool IsInputWeightDirty { get; set; }


        public PrepareFrameArgs(IReadOnlyList<NodeBase> inputNodes, IReadOnlyList<float> inputWeights)
        {
            InputNodes = inputNodes;
            InputWeights = inputWeights;
        }
    }
}
