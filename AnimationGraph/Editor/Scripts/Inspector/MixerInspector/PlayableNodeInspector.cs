using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class PlayableNodeInspector : GraphElementInspector<GraphNode>
    {
        protected new PlayableNode Target => (PlayableNode)base.Target;

        protected ParamField SpeedParamField { get; }


        protected PlayableNodeInspector()
        {
            // Speed
            SpeedParamField = new ParamField(FieldLabelWidth);
            SpeedParamField.OnParamChanged += OnSpeedChanged;
            SpeedParamField.OnToggleActive += OnSpeedActiveChanged;
            Add(SpeedParamField);
        }


        public override void SetTarget(GraphNode target)
        {
            base.SetTarget(target);

            // Speed
            SpeedParamField.SetParamTarget("Speed", Target.NodeData.SpeedParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                Target.NodeData.SpeedParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                Target.NodeData.SpeedParamActive ? ParamActiveState.Active : ParamActiveState.Inactive, null);
        }


        private void OnSpeedChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSpeedActiveChanged(bool isActive)
        {
            Target.NodeData.SpeedParamActive = isActive;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
