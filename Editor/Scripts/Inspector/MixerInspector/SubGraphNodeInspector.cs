using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class SubGraphNodeInspector : PlayableNodeInspector
    {
        private SubGraphNodeData NodeData => (SubGraphNodeData)Target.NodeData;

        private readonly ObjectField _subGraphField;


        public SubGraphNodeInspector()
        {
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
        }

        public override void SetTarget(GraphNode target)
        {
            base.SetTarget(target);

            // Sub graph
            _subGraphField.SetValueWithoutNotify(NodeData.SubGraph);
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
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
