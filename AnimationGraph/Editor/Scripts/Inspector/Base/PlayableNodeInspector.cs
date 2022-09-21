using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class PlayableNodeInspector : GraphNodeInspector
    {
        protected new PlayableNode TargetNode => (PlayableNode)base.TargetNode;

        protected ParamField SpeedParamField { get; }

        
        protected PlayableNodeInspector()
        {
            // Speed
            SpeedParamField = new ParamField(FieldLabelWidth);
            SpeedParamField.OnParamChanged += OnSpeedChanged;
            SpeedParamField.OnToggleActive += OnSpeedActiveChanged;
            Add(SpeedParamField);
        }

        
        public override void SetTargetNode(GraphNode node)
        {
            base.SetTargetNode(node);

            // Speed
            SpeedParamField.SetParamTarget("Speed", TargetNode.NodeData.SpeedParam, ParamType.Float,
                TargetNode.GraphAsset.Parameters, true, TargetNode.NodeData.SpeedParamActive, null);
        }


        private void OnSpeedChanged(ParamGuidOrValue _)
        {
            RaiseParamChangedEvent();
        }

        private void OnSpeedActiveChanged(bool isActive)
        {
            TargetNode.NodeData.SpeedParamActive = isActive;
            RaiseParamChangedEvent();
        }
    }
}
