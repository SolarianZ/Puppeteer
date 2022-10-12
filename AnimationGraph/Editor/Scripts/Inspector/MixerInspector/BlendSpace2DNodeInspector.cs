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
    public class BlendSpace2DSampleDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 88;

        public event Action OnDataChanged;


        private readonly ObjectField _clipField;

        private readonly Vector2Field _positionField;

        private readonly FloatField _playbackSpeedField;

        private BlendSpace2DSample _target;


        public BlendSpace2DSampleDrawer(Length nameLabelWidth)
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
            _positionField = new Vector2Field("Position");
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

        public void SetTarget(BlendSpace2DSample target)
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

        private void OnPositionChanged(ChangeEvent<Vector2> evt)
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
    public class BlendSpace2DNodeInspector : PlayableNodeInspector
    {
        private readonly ParamField _positionXParamField;

        private readonly ParamField _positionYParamField;

        private readonly ListView _sampleListView;

        private BlendSpace2DNodeData _nodeData;


        public BlendSpace2DNodeInspector()
        {
            // Position X param
            _positionXParamField = new ParamField(FieldLabelWidth);
            _positionXParamField.OnParamChanged += OnPositionChanged;
            Add(_positionXParamField);

            // Position Y param
            _positionYParamField = new ParamField(FieldLabelWidth);
            _positionYParamField.OnParamChanged += OnPositionChanged;
            Add(_positionYParamField);

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
                fixedItemHeight = BlendSpace2DSampleDrawer.DRAWER_HEIGHT,
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

            _nodeData = (BlendSpace2DNodeData)((BlendSpace2DNode)target).NodeData;

            // Position X param
            _positionXParamField.SetParamTarget("Position X", _nodeData.PositionXParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _nodeData.PositionXParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);

            // Position Y param
            _positionYParamField.SetParamTarget("Position Y", _nodeData.PositionYParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _nodeData.PositionYParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
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
            var drawer = new BlendSpace2DSampleDrawer(FieldLabelWidth);
            drawer.OnDataChanged += () => RaiseDataChangedEvent(DataCategories.NodeData);

            return drawer;
        }

        private void BindSampleListItem(VisualElement element, int index)
        {
            var drawer = (BlendSpace2DSampleDrawer)element;
            drawer.SetTarget(_nodeData.Samples[index]);
        }

        private void OnSampleIndexChanged(int from, int to)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _nodeData.Samples[index] = new BlendSpace2DSample();
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemRemoved(IEnumerable<int> indices)
        {
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
