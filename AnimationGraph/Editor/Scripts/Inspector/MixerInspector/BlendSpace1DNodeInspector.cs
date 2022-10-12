using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class BlendSpace1DSampleDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 66;

        public event Action OnDataChanged;


        private readonly ObjectField _clipField;

        private readonly FloatField _positionField;

        private readonly FloatField _playbackSpeedField;

        private BlendSpace1DSample _target;


        public BlendSpace1DSampleDrawer(Length nameLabelWidth)
        {
            // Clip
            _clipField = new ObjectField("Clip")
            {
                objectType = typeof(AnimationClip),
            };
            _clipField.labelElement.style.minWidth = StyleKeyword.Auto;
            _clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _clipField.labelElement.style.width = nameLabelWidth;
            _clipField.RegisterValueChangedCallback(OnClipChanged);
            Add(_clipField);

            // Position
            _positionField = new FloatField("Position");
            _positionField.labelElement.style.minWidth = StyleKeyword.Auto;
            _positionField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _positionField.labelElement.style.width = nameLabelWidth;
            _positionField.RegisterValueChangedCallback(OnPositionChanged);
            Add(_positionField);

            // Playback speed
            _playbackSpeedField = new FloatField("Speed");
            _playbackSpeedField.labelElement.style.minWidth = StyleKeyword.Auto;
            _playbackSpeedField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _playbackSpeedField.labelElement.style.width = nameLabelWidth;
            _playbackSpeedField.RegisterValueChangedCallback(OnPlaybackSpeedChanged);
            Add(_playbackSpeedField);
        }

        public void SetTarget(BlendSpace1DSample target)
        {
            _target = target;

            _clipField.SetValueWithoutNotify(_target.Clip);
            _positionField.SetValueWithoutNotify(_target.Position);
            _playbackSpeedField.SetValueWithoutNotify(_target.PlaybackSpeed);
        }


        private void OnClipChanged(ChangeEvent<UObject> evt)
        {
            _target.Clip = (AnimationClip)evt.newValue;
            OnDataChanged?.Invoke();
        }

        private void OnPositionChanged(ChangeEvent<float> evt)
        {
            _target.Position = evt.newValue;
            OnDataChanged?.Invoke();
        }

        private void OnPlaybackSpeedChanged(ChangeEvent<float> evt)
        {
            _target.PlaybackSpeed = evt.newValue;
            OnDataChanged?.Invoke();
        }
    }

    // TODO: Validate samples position
    // TODO: Visual preview
    public class BlendSpace1DNodeInspector : PlayableNodeInspector
    {
        private readonly ParamField _positionParamField;

        private readonly ListView _sampleListView;

        private BlendSpace1DNodeData _nodeData;


        public BlendSpace1DNodeInspector()
        {
            // Position param
            _positionParamField = new ParamField(FieldLabelWidth);
            _positionParamField.OnParamChanged += OnPositionChanged;
            Add(_positionParamField);

            // Samples
            var sampleListViewLabel = new Label("Samples")
            {
                style =
                {
                    height = 20,
                    marginLeft = 3,
                    marginRight = 3,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(sampleListViewLabel);
            _sampleListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = BlendSpace1DSampleDrawer.DRAWER_HEIGHT,
                makeItem = MakeSampleListItem,
                bindItem = BindSampleListItem,
                selectionType = SelectionType.Single,
                showAddRemoveFooter = true,
            };
            _sampleListView.itemIndexChanged += OnSampleIndexChanged;
            _sampleListView.itemsAdded += OnSampleItemAdded;
            _sampleListView.itemsRemoved += OnSampleItemRemoved;
            Add(_sampleListView);
        }

        public override void SetTarget(GraphNode target)
        {
            base.SetTarget(target);

            _nodeData = (BlendSpace1DNodeData)((BlendSpace1DNode)target).NodeData;

            // Position param
            _positionParamField.SetParamTarget("Position", _nodeData.PositionParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _nodeData.PositionParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);

            // Samples
            _sampleListView.itemsSource = _nodeData.Samples;
            _sampleListView.RefreshItems();
        }

        private void OnPositionChanged(ParamGuidOrValue _)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private VisualElement MakeSampleListItem()
        {
            var drawer = new BlendSpace1DSampleDrawer(FieldLabelWidth);
            drawer.OnDataChanged += () => RaiseDataChangedEvent(DataCategories.NodeData);

            return drawer;
        }

        private void BindSampleListItem(VisualElement element, int index)
        {
            var drawer = (BlendSpace1DSampleDrawer)element;
            drawer.SetTarget(_nodeData.Samples[index]);
        }

        private void OnSampleIndexChanged(int from, int to)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _nodeData.Samples[index] = new BlendSpace1DSample();
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemRemoved(IEnumerable<int> indices)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
