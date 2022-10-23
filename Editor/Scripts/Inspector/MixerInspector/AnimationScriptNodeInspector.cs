using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Node;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationScriptNodeInspector : PlayableNodeInspector
    {
        private AnimationScriptNode Node => (AnimationScriptNode)Target.Node;

        private readonly ObjectField _scriptAssetField;


        public AnimationScriptNodeInspector()
        {
            // Script asset
            _scriptAssetField = new ObjectField("Script Asset")
            {
                objectType = typeof(AnimationScriptAsset),
            };
            _scriptAssetField.labelElement.style.minWidth = StyleKeyword.Auto;
            _scriptAssetField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _scriptAssetField.labelElement.style.width = FieldLabelWidth;
            _scriptAssetField.RegisterValueChangedCallback(OnScriptAssetChanged);
            Add(_scriptAssetField);
        }

        public override void SetTarget(GraphEditorNode target)
        {
            base.SetTarget(target);

            // Script asset
            _scriptAssetField.SetValueWithoutNotify(Node.ScriptAsset);
        }

        private void OnScriptAssetChanged(ChangeEvent<UObject> evt)
        {
            Node.ScriptAsset = (AnimationScriptAsset)evt.newValue;
            RaiseDataChangedEvent(DataCategories.NodeData);
        }
    }
}
