using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationMixerNodeInspector : PlayableNodeInspector
    {
        protected List<ParamInfo> ParamTable { get; }

        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly ListView _inputListView;

        private List<MixerInputData> _mixerInputs;


        public AnimationMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement,
            float inputDrawerHeight = MixerInputDataDrawer.DRAWER_HEIGHT)
        {
            ParamTable = paramTable;
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
                fixedItemHeight = inputDrawerHeight,
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

        public override void SetTarget(GraphNode target)
        {
            base.SetTarget(target);

            _mixerInputs = Target.NodeData.MixerInputs;
            _inputListView.itemsSource = _mixerInputs;
            _inputListView.RefreshItems();
        }

        public void RefreshMixerInputList()
        {
            _inputListView.RefreshItems();
        }


        protected virtual MixerInputDataDrawer CreateMixerInputDataDrawer()
        {
            return new MixerInputDataDrawer(ParamTable, FieldLabelWidth);
        }

        protected virtual MixerInputData CreateMixerInputData()
        {
            return new MixerInputData();
        }


        private VisualElement MakeInputListItem()
        {
            var mixerDrawer = CreateMixerInputDataDrawer();
            mixerDrawer.OnDataChanged += () => RaiseDataChangedEvent(DataCategories.NodeData);

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
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnInputItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _mixerInputs[index] = CreateMixerInputData();
            _addInputPortElement(index);
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private void OnInputItemRemoved(IEnumerable<int> indices)
        {
            _removeInputPortElement(indices.First());
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
