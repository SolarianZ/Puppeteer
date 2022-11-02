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
    public class ScriptNodeInspector : PlayableNodeInspector
    {
        private ScriptNode Node => (ScriptNode)Target.Node;

        private List<ParamInfo> ParamTable { get; }

        private readonly Action<int> _addInputPortElement;

        private readonly Action<int> _removeInputPortElement;

        private readonly Action<int, int> _reorderInputPortElement;

        private readonly ObjectField _scriptAssetField;

        private readonly ListView _inputListView;

        private List<WeightedNodeInput> _mixerInputs;


        public ScriptNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement)
        {
            ParamTable = paramTable;
            _addInputPortElement = addInputPortElement;
            _removeInputPortElement = removeInputPortElement;
            _reorderInputPortElement = reorderInputPortElement;

            // Script asset
            _scriptAssetField = new ObjectField("Script Asset")
            {
                objectType = typeof(ScriptAsset),
            };
            _scriptAssetField.labelElement.style.minWidth = StyleKeyword.Auto;
            _scriptAssetField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _scriptAssetField.labelElement.style.width = FieldLabelWidth;
            _scriptAssetField.RegisterValueChangedCallback(OnScriptAssetChanged);
            Add(_scriptAssetField);

            // Mixer inputs
            var inputListViewLabel = new Label("Inputs")
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
                fixedItemHeight = WeightedNodeInputDrawer.DRAWER_HEIGHT,
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

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            // Script asset
            _scriptAssetField.SetValueWithoutNotify(Node.Script);

            // Inputs
            _mixerInputs = ((ScriptEditorNode)Target).Node.Inputs;
            _inputListView.itemsSource = _mixerInputs;
            _inputListView.RefreshItems();
        }

        public void RefreshMixerInputList()
        {
            _inputListView.RefreshItems();
        }


        private void OnScriptAssetChanged(ChangeEvent<UObject> evt)
        {
            Node.Script = (ScriptAsset)evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private VisualElement MakeInputListItem()
        {
            var drawer = new WeightedNodeInputDrawer(ParamTable, FieldLabelWidth);
            drawer.OnDataChanged += () => RaiseDataChangedEvent(DataCategories.NodeData);

            return drawer;
        }

        private void BindInputListItem(VisualElement element, int index)
        {
            var drawer = (WeightedNodeInputDrawer)element;
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
            _mixerInputs[index] = new WeightedNodeInput();
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
