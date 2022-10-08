using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class LayerMixerInputDataDrawer : MixerInputDataDrawer
    {
        public new const float DRAWER_HEIGHT = 90;

        private readonly Toggle _isAdditiveField;

        private readonly ObjectField _avatarMaskField;

        private LayerMixerInputData _layerMixerInputData;


        public LayerMixerInputDataDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
            : base(paramTable, nameLabelWidth)
        {
            style.height = DRAWER_HEIGHT;

            _isAdditiveField = new Toggle("Is Additive");
            _isAdditiveField.labelElement.style.minWidth = StyleKeyword.Auto;
            _isAdditiveField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _isAdditiveField.labelElement.style.width = nameLabelWidth;
            _isAdditiveField.labelElement.style.overflow = Overflow.Hidden;
            _isAdditiveField.RegisterValueChangedCallback(OnAdditiveChanged);
            Add(_isAdditiveField);

            _avatarMaskField = new ObjectField("Avatar Mask")
            {
                objectType = typeof(AvatarMask),
            };
            _avatarMaskField.labelElement.style.minWidth = StyleKeyword.Auto;
            _avatarMaskField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _avatarMaskField.labelElement.style.width = nameLabelWidth;
            _avatarMaskField.labelElement.style.overflow = Overflow.Hidden;
            _avatarMaskField.RegisterValueChangedCallback(OnAvatarMaskChanged);
            Add(_avatarMaskField);
        }

        public override void SetMixerInputData(MixerInputData mixerInputData, int mixerInputDataIndex)
        {
            base.SetMixerInputData(mixerInputData, mixerInputDataIndex);

            _layerMixerInputData = (LayerMixerInputData)mixerInputData;

            _isAdditiveField.SetValueWithoutNotify(_layerMixerInputData.IsAdditive);

            _avatarMaskField.SetValueWithoutNotify(_layerMixerInputData.AvatarMask);
        }


        private void OnAdditiveChanged(ChangeEvent<bool> evt)
        {
            if (_layerMixerInputData != null)
            {
                _layerMixerInputData.IsAdditive = evt.newValue;
            }
        }

        private void OnAvatarMaskChanged(ChangeEvent<UObject> evt)
        {
            if (_layerMixerInputData != null)
            {
                _layerMixerInputData.AvatarMask = (AvatarMask)evt.newValue;
            }
        }
    }

    public class AnimationLayerMixerNodeInspector : PlayableNodeInspector
    {
        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly List<ParamInfo> _paramTable;

        private readonly ListView _inputListView;

        private List<MixerInputData> _mixerInputs;


        public AnimationLayerMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement)
        {
            _paramTable = paramTable;
            _addInputPortElement = addInputPortElement;
            _removeInputPortElement = removeInputPortElement;
            _reorderInputPortElement = reorderInputPortElement;

            // Mixer inputs
            var inputListViewLabel = new Label("Mixer Inputs")
            {
                style =
                {
                    height = 20,
                    marginLeft = 3,
                    marginRight = 3,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(inputListViewLabel);
            _inputListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = LayerMixerInputDataDrawer.DRAWER_HEIGHT,
                makeItem = MakeInputListItem,
                bindItem = BindInputListItem,
                selectionType = SelectionType.Single,
                showAddRemoveFooter = true,
            };
            _inputListView.itemIndexChanged += OnInputIndexChanged;
            _inputListView.itemsAdded += OnInputItemAdded;
            _inputListView.itemsRemoved += OnInputItemRemoved;
            Add(_inputListView);
        }

        public override void SetTarget(GraphNode node)
        {
            base.SetTarget(node);

            _mixerInputs = Target.NodeData.MixerInputs;
            _inputListView.itemsSource = _mixerInputs;
            _inputListView.RefreshItems();
        }

        public void RefreshMixerInputList()
        {
            _inputListView.RefreshItems();
        }


        private VisualElement MakeInputListItem()
        {
            var mixerDrawer = new LayerMixerInputDataDrawer(_paramTable, FieldLabelWidth);
            mixerDrawer.OnWeightChanged += OnMixerInputWeightChanged;

            return mixerDrawer;
        }

        private void BindInputListItem(VisualElement element, int index)
        {
            var drawer = (LayerMixerInputDataDrawer)element;
            drawer.SetMixerInputData(_mixerInputs[index], index);
        }

        private void OnInputIndexChanged(int from, int to)
        {
            _reorderInputPortElement(from, to);
            RaiseParamChangedEvent();
        }

        private void OnInputItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _mixerInputs[index] = new LayerMixerInputData();
            _addInputPortElement(index);
            RaiseParamChangedEvent();
        }

        private void OnInputItemRemoved(IEnumerable<int> indices)
        {
            _removeInputPortElement(indices.First());
            RaiseParamChangedEvent();
        }

        private void OnMixerInputWeightChanged(ParamGuidOrValue param)
        {
            RaiseParamChangedEvent();
        }
    }
}
