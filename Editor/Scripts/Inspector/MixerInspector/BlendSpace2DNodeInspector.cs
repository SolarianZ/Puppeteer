using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BlendSpace2DSampleDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 66;

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

        public void SetTarget(BlendSpace2DSample target, int targetIndex)
        {
            _target = target;

            _clipField.SetValueWithoutNotify(_target.Clip);
            _positionField.SetValueWithoutNotify(_target.Position);
            _playbackSpeedField.SetValueWithoutNotify(_target.Speed);
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
            _target.Speed = evt.newValue;
            OnDataChanged?.Invoke();
        }
    }

    // TODO: Validate samples position
    // TODO: Visual preview
    public class BlendSpace2DNodeInspector : PlayableNodeInspector
    {
        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly ParamField _positionXParamField;

        private readonly ParamField _positionYParamField;

        private readonly ListView _sampleListView;

        private BlendSpace2DNode _node;


        public BlendSpace2DNodeInspector(Action<int> addInputPortElement, Action<int> removeInputPortElement,
            Action<int, int> reorderInputPortElement)
        {
            _addInputPortElement = addInputPortElement;
            _removeInputPortElement = removeInputPortElement;
            _reorderInputPortElement = reorderInputPortElement;

            // Position X param
            _positionXParamField = new ParamField(FieldLabelWidth);
            _positionXParamField.OnParamChanged += OnPositionChanged;
            Add(_positionXParamField);

            // Position Y param
            _positionYParamField = new ParamField(FieldLabelWidth);
            _positionYParamField.OnParamChanged += OnPositionChanged;
            Add(_positionYParamField);

            // Inputs
            var sampleListViewLabel = new Label("Inputs")
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

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            _node = ((BlendSpace2DEditorNode)target).Node;

            // Position X param
            _positionXParamField.SetParamTarget("Position X", _node.PositionXParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _node.PositionXParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);

            // Position Y param
            _positionYParamField.SetParamTarget("Position Y", _node.PositionYParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _node.PositionYParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);

            // Samples
            _sampleListView.itemsSource = _node.Samples;
            _sampleListView.RefreshItems();
        }

        public void RefreshSampleInputList()
        {
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
            drawer.SetTarget(_node.Samples[index], index);
        }

        private void OnSampleIndexChanged(int from, int to)
        {
            _reorderInputPortElement(from, to);
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _node.Samples[index] = new BlendSpace2DSample();
            _addInputPortElement(index);
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnSampleItemRemoved(IEnumerable<int> indices)
        {
            _removeInputPortElement(indices.First());
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
