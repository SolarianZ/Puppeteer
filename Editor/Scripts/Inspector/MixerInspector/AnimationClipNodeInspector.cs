using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationClipNodeInspector : PlayableNodeInspector
    {
        private AnimationClipNode Node => (AnimationClipNode)Target.Node;

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
            _motionTimeParamField.OnActivityChanged += OnMotionTimeActiveChanged;
            Add(_motionTimeParamField);

            // Cycle Offset
            // _cycleOffsetParamField = new ParamField(FieldLabelWidth);
            // _cycleOffsetParamField.OnParamChanged += OnCycleOffsetChanged;
            // _cycleOffsetParamField.OnActivityChanged += OnCycleOffsetActivityChanged;
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

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            // Motion Time
            _motionTimeParamField.SetParamTarget("Motion Time", Node.MotionTimeParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                Node.MotionTimeParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                Node.MotionTimeParamActive ? ParamActiveState.Active : ParamActiveState.Inactive, null);

            // Cycle Offset
            // _cycleOffsetParamField.SetParamTarget("Cycle Offset", ClipNodeData.CycleOffsetParam, ParamType.Float,
            //     TargetNode.GraphAsset.Parameters, true, ClipNodeData.CycleOffsetParamActive, new Vector2(0, 1));

            // Clip
            _clipField.SetValueWithoutNotify(Node.Clip);

            // FootIK
            _footIKField.SetValueWithoutNotify(Node.ApplyFootIK);

            // PlayableIK
            _playableIKField.SetValueWithoutNotify(Node.ApplyPlayableIK);
        }


        private void OnMotionTimeChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnMotionTimeActiveChanged(bool isActive)
        {
            Node.MotionTimeParamActive = isActive;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        // private void OnCycleOffsetChanged(ParamGuidOrValue _)
        // {
        //     RaiseDataChangedEvent(DataCategories.NodeData);
        // }
        //
        // private void OnCycleOffsetActivityChanged(bool isActive)
        // {
        //     NodeData.CycleOffsetParamActive = isActive;
        //     RaiseDataChangedEvent(DataCategories.NodeData);
        // }

        private void OnClipChanged(ChangeEvent<UObject> evt)
        {
            Node.Clip = (AnimationClip)evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnFootIKParamChanged(ChangeEvent<bool> evt)
        {
            Node.ApplyFootIK = evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnPlayableIKParamChanged(ChangeEvent<bool> evt)
        {
            Node.ApplyPlayableIK = evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
