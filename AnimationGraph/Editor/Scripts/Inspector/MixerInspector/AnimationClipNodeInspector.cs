using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationClipNodeInspector : PlayableNodeInspector
    {
        private AnimationClipNodeData NodeData => (AnimationClipNodeData)Target.NodeData;

        private readonly ObjectField _clipField;

        private readonly ParamField _motionTimeParamField;

        // private readonly ParamField _cycleOffsetParamField;

        private readonly Toggle _footIKField;

        private readonly Toggle _playableIKField;


        public AnimationClipNodeInspector()
        {
            // Motion Time
            _motionTimeParamField = new ParamField(FieldLabelWidth);
            _motionTimeParamField.OnParamChanged += OnMotionTimeChanged;
            _motionTimeParamField.OnToggleActive += OnMotionTimeActiveChanged;
            Add(_motionTimeParamField);

            // Cycle Offset
            // _cycleOffsetParamField = new ParamField(FieldLabelWidth);
            // _cycleOffsetParamField.OnParamChanged += OnCycleOffsetChanged;
            // _cycleOffsetParamField.OnToggleActive += OnCycleOffsetActiveChanged;
            // Add(_cycleOffsetParamField);

            // Clip
            _clipField = new ObjectField("Clip")
            {
                objectType = typeof(AnimationClip),
            };
            _clipField.labelElement.style.minWidth = StyleKeyword.Auto;
            _clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _clipField.labelElement.style.width = FieldLabelWidth;
            _clipField.RegisterValueChangedCallback(OnClipChanged);
            Add(_clipField);

            // FootIK
            _footIKField = new Toggle("Foot IK");
            _footIKField.labelElement.style.minWidth = StyleKeyword.Auto;
            _footIKField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _footIKField.labelElement.style.width = FieldLabelWidth;
            _footIKField.RegisterValueChangedCallback(OnFootIKParamChanged);
            Add(_footIKField);

            // PlayableIK
            _playableIKField = new Toggle("Playable IK");
            _playableIKField.labelElement.style.minWidth = StyleKeyword.Auto;
            _playableIKField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _playableIKField.labelElement.style.width = FieldLabelWidth;
            _playableIKField.RegisterValueChangedCallback(OnPlayableIKParamChanged);
            Add(_playableIKField);
        }

        public override void SetTarget(GraphNode node)
        {
            base.SetTarget(node);

            // Motion Time
            _motionTimeParamField.SetParamTarget("Motion Time", NodeData.MotionTimeParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                NodeData.MotionTimeParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                NodeData.MotionTimeParamActive ? ParamActiveState.Active : ParamActiveState.Inactive, null);

            // Cycle Offset
            // _cycleOffsetParamField.SetParamTarget("Cycle Offset", ClipNodeData.CycleOffsetParam, ParamType.Float,
            //     TargetNode.GraphAsset.Parameters, true, ClipNodeData.CycleOffsetParamActive, new Vector2(0, 1));

            // Clip
            _clipField.SetValueWithoutNotify(NodeData.Clip);

            // FootIK
            _footIKField.SetValueWithoutNotify(NodeData.ApplyFootIK);

            // PlayableIK
            _playableIKField.SetValueWithoutNotify(NodeData.ApplyPlayableIK);
        }


        private void OnMotionTimeChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnMotionTimeActiveChanged(bool isActive)
        {
            NodeData.MotionTimeParamActive = isActive;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void OnCycleOffsetChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnCycleOffsetActiveChanged(bool isActive)
        {
            NodeData.CycleOffsetParamActive = isActive;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnClipChanged(ChangeEvent<Object> evt)
        {
            NodeData.Clip = (AnimationClip)evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnFootIKParamChanged(ChangeEvent<bool> evt)
        {
            NodeData.ApplyFootIK = evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnPlayableIKParamChanged(ChangeEvent<bool> evt)
        {
            NodeData.ApplyPlayableIK = evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
