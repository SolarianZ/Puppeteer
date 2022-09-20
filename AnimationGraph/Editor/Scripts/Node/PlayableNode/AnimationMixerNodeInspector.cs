using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Node
{
    public class MixerInputDataDrawer : VisualElement
    {
        public event Action<ParamGuidOrValue> OnWeightChanged;


        private readonly List<ParamInfo> _paramTable;

        private readonly TextField _inputNodeField;

        private readonly ParamField _inputWeightParamField;


        public MixerInputDataDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
        {
            _paramTable = paramTable;

            _inputNodeField = new TextField("Input Node");
            _inputNodeField.labelElement.style.minWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.width = nameLabelWidth;
            _inputNodeField.SetEnabled(false);
            Add(_inputNodeField);

            _inputWeightParamField = new ParamField(nameLabelWidth);
            _inputWeightParamField.OnParamChanged += RaiseMixerInputDataChangedEvent;
            Add(_inputWeightParamField);
        }

        public void SetMixerInputData(MixerInputData mixerInputData, int mixerInputDataIndex)
        {
            _inputNodeField.label = $"Input Node {mixerInputDataIndex.ToString()}";
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
        private readonly Action<int> _addInputPort;

        private readonly Action<int> _removeInputPort;

        private readonly Action<int, int> _reorderInputPort;

        private readonly List<ParamInfo> _paramTable;

        private readonly ListView _inputListView;

        private List<MixerInputData> _mixerInputs;


        public AnimationMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPort,
            Action<int> removeInputPort, Action<int, int> reorderInputPort)
        {
            _paramTable = paramTable;
            _addInputPort = addInputPort;
            _removeInputPort = removeInputPort;
            _reorderInputPort = reorderInputPort;

            var inputListViewLabel = new Label("Mixer Inputs");
            Add(inputListViewLabel);
            _inputListView = new ListView
            {
                headerTitle = "Mixer Inputs",
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = 44,
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
            // var drawer = (MixerInputDataDrawer)element;
            // drawer.SetMixerInputData(_mixerInputs[index], index);
        }

        private void OnInputIndexChanged(int from, int to)
        {
        }

        private void OnInputItemRemoved(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                Debug.Log(index);
            }
        }

        private void OnInputItemAdded(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                Debug.Log(index);
            }
        }

        private void OnMixerInputWeightChanged(ParamGuidOrValue param)
        {
            RaiseParamChangedEvent();
        }
    }
}
