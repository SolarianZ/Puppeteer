using System.Collections.Generic;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class SubGraphNodeInspector : PlayableNodeInspector
    {
        protected List<ParamInfo> ParamTable { get; }

        private SubGraphNodeData NodeData => (SubGraphNodeData)Target.NodeData;

        private readonly ObjectField _subGraphField;

        private readonly ListView _paramBindingListView;


        public SubGraphNodeInspector(List<ParamInfo> paramTable)
        {
            ParamTable = paramTable;

            // Sub graph
            _subGraphField = new ObjectField("Sub Graph")
            {
                objectType = typeof(AnimationGraphAsset),
            };
            _subGraphField.labelElement.style.minWidth = StyleKeyword.Auto;
            _subGraphField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _subGraphField.labelElement.style.width = FieldLabelWidth;
            _subGraphField.RegisterValueChangedCallback(OnSubGraphChanged);
            Add(_subGraphField);

            // Param bindings
            var paramBindingListViewLabel = new Label("Param Bindings")
            {
                style =
                {
                    height = 20,
                    marginLeft = 3,
                    marginRight = 3,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(paramBindingListViewLabel);
            _paramBindingListView = new ListView
            {
                reorderable = false,
                fixedItemHeight = ParamField.MIN_ELEMENT_HEIGHT,
                makeItem = MakeParamBindingListItem,
                bindItem = BindParamBindingListItem,
                selectionType = SelectionType.Single,
            };
            Add(_paramBindingListView);
        }

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            // Sub graph
            var subGraph = NodeData.SubGraph;
            _subGraphField.SetValueWithoutNotify(subGraph);

            // Param bindings
            ResetParamBindings();
            _paramBindingListView.itemsSource = NodeData.ParamBindings;
        }

        private void ResetParamBindings()
        {
            NodeData.ParamBindings.Clear();
            if (!NodeData.SubGraph)
            {
                return;
            }

            foreach (var param in NodeData.SubGraph.Parameters)
            {
                NodeData.ParamBindings.Add(new ParamBindingGuidOrValue(0, param.Guid));
            }
        }

        private void OnSubGraphChanged(ChangeEvent<UObject> evt)
        {
            var oldSubGraph = NodeData.SubGraph;
            var newSubGraph = (AnimationGraphAsset)evt.newValue;
            if (newSubGraph == Target.GraphAsset)
            {
                _subGraphField.SetValueWithoutNotify(oldSubGraph);

                var msg = "Circular references are not supported, please select another graph asset.";
                EditorUtility.DisplayDialog("Error", msg, "OK");

                return;
            }

            NodeData.SubGraph = (AnimationGraphAsset)evt.newValue;

            ResetParamBindings();
            _paramBindingListView.RefreshItems();

            RaiseDataChangedEvent(DataCategories.NodeData);
        }

        private VisualElement MakeParamBindingListItem()
        {
            var srcParamField = new ParamField(FieldLabelWidth);
            srcParamField.OnParamChanged += (_) => RaiseDataChangedEvent(DataCategories.NodeData);

            return srcParamField;
        }

        private void BindParamBindingListItem(VisualElement element, int index)
        {
            var srcParamField = (ParamField)element;
            var destParamInfo = NodeData.SubGraph.Parameters[index];
            srcParamField.SetParamTarget(destParamInfo.Name,
                NodeData.ParamBindings[index].SrcParamGuidOrValue,
                destParamInfo.Type, ParamTable,
                NodeData.ParamBindings[index].IsValue() ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);
        }
    }
}
