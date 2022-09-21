using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class MixerInputDataDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 44;

        public event Action<ParamGuidOrValue> OnWeightChanged;


        private readonly List<ParamInfo> _paramTable;

        private readonly TextField _inputNodeField;

        private readonly ParamField _inputWeightParamField;


        public MixerInputDataDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
        {
            _paramTable = paramTable;

            style.height = DRAWER_HEIGHT;
            style.justifyContent = Justify.SpaceAround;

            _inputNodeField = new TextField("Input Node");
            _inputNodeField.labelElement.style.minWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.width = nameLabelWidth;
            _inputNodeField.labelElement.style.overflow = Overflow.Hidden;
            _inputNodeField.SetEnabled(false);
            Add(_inputNodeField);

            _inputWeightParamField = new ParamField(nameLabelWidth);
            _inputWeightParamField.OnParamChanged += RaiseMixerInputDataChangedEvent;
            Add(_inputWeightParamField);
        }

        public void SetMixerInputData(MixerInputData mixerInputData, int mixerInputDataIndex)
        {
            _inputNodeField.label = $"Input Node {mixerInputDataIndex.ToString()}";
            _inputNodeField.SetValueWithoutNotify(mixerInputData.InputNodeGuid);

            _inputWeightParamField.SetParamTarget($"Input Weight {mixerInputDataIndex.ToString()}",
                mixerInputData.InputWeightParam, ParamType.Float, _paramTable, true, null, new Vector2(0, 1));
        }


        protected void RaiseMixerInputDataChangedEvent(ParamGuidOrValue param)
        {
            OnWeightChanged?.Invoke(param);
        }
    }

    public class AnimationMixerNodeInspector : PlayableNodeInspector
    {
        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly List<ParamInfo> _paramTable;

        private readonly ListView _inputListView;

        private List<MixerInputData> _mixerInputs;


        public AnimationMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement)
        {
            _paramTable = paramTable;
            _addInputPortElement = addInputPortElement;
            _removeInputPortElement = removeInputPortElement;
            _reorderInputPortElement = reorderInputPortElement;

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
                fixedItemHeight = MixerInputDataDrawer.DRAWER_HEIGHT,
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

        public override void SetTargetNode(GraphNode node)
        {
            base.SetTargetNode(node);

            _mixerInputs = ((PlayableNode)node).NodeData.MixerInputs;
            _inputListView.itemsSource = _mixerInputs;
            _inputListView.RefreshItems();
        }

        public void RefreshMixerInputList()
        {
            _inputListView.RefreshItems();
        }


        private VisualElement MakeInputListItem()
        {
            var mixerDrawer = new MixerInputDataDrawer(_paramTable, FieldLabelWidth);
            mixerDrawer.OnWeightChanged += OnMixerInputWeightChanged;

            return mixerDrawer;
        }

        private void BindInputListItem(VisualElement element, int index)
        {
            var drawer = (MixerInputDataDrawer)element;
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
            _mixerInputs[index] = new MixerInputData();
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
