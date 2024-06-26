﻿using System;
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

        public void SetTarget(BlendSpace1DSample target, int targetIndex)
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

        private void OnPositionChanged(ChangeEvent<float> evt)
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
    public class BlendSpace1DNodeInspector : PlayableNodeInspector
    {
        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly ParamField _positionParamField;

        private readonly ListView _sampleListView;

        private BlendSpace1DNode _node;


        public BlendSpace1DNodeInspector(Action<int> addInputPortElement, Action<int> removeInputPortElement,
            Action<int, int> reorderInputPortElement)
        {
            _addInputPortElement = addInputPortElement;
            _removeInputPortElement = removeInputPortElement;
            _reorderInputPortElement = reorderInputPortElement;

            // Position param
            _positionParamField = new ParamField(FieldLabelWidth);
            _positionParamField.OnParamChanged += OnPositionChanged;
            Add(_positionParamField);

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

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            _node = ((BlendSpace1DEditorNode)target).Node;

            // Position param
            _positionParamField.SetParamTarget("Position", _node.PositionParam,
                ParamType.Float, Target.GraphAsset.Parameters,
                _node.PositionParam.IsLiteral ? ParamLinkState.Unlinked : ParamLinkState.Linked,
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
            var drawer = new BlendSpace1DSampleDrawer(FieldLabelWidth);
            drawer.OnDataChanged += () => RaiseDataChangedEvent(DataCategories.NodeData);

            return drawer;
        }

        private void BindSampleListItem(VisualElement element, int index)
        {
            var drawer = (BlendSpace1DSampleDrawer)element;
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
            _node.Samples[index] = new BlendSpace1DSample();
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
