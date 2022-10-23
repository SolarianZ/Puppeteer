using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class PlayableNodeInspector : GraphElementInspector<GraphEditorNode>
    {
        protected new MixerGraphEditorNode Target => (MixerGraphEditorNode)base.Target;

        protected ParamField SpeedParamField { get; }


        protected PlayableNodeInspector()
        {
            // Speed
            SpeedParamField = new ParamField(FieldLabelWidth);
            SpeedParamField.OnParamChanged += OnSpeedChanged;
            SpeedParamField.OnActivityChanged += OnSpeedActiveChanged;
            Add(SpeedParamField);
        }


        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            // Speed
            SpeedParamField.SetParamTarget("Speed", Target.Node.SpeedParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                Target.Node.SpeedParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                Target.Node.SpeedParamActive ? ParamActiveState.Active : ParamActiveState.Inactive, null);
        }


        private void OnSpeedChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSpeedActiveChanged(bool isActive)
        {
            Target.Node.SpeedParamActive = isActive;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
