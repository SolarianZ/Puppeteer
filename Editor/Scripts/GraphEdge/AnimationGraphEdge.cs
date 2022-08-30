using GBG.Puppeteer.Editor.GraphPort;
using GraphViewEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.Puppeteer.Editor.GraphEdge
{
    public class AnimationGraphEdge : GraphViewEdge
    {
        public AnimationGraphPort Input => input as AnimationGraphPort;

        public AnimationGraphPort Output => output as AnimationGraphPort;
    }
}
