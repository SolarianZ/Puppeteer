using GBG.AnimationGraph.Editor.Node;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class PlayableNodeInspector : GraphElementInspector<GraphEditorNode>
    {
        protected new MixerGraphEditorNode Target => (MixerGraphEditorNode)base.Target;
    }
}
