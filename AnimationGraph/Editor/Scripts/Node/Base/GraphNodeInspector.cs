using System;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Node
{
    public class GraphNodeInspector : VisualElement
    {
        public event Action OnParamChanged;


        protected Length FieldLabelWidth { get; set; } = Length.Percent(35);

        protected GraphNode TargetNode { get; private set; }

        protected TextField NodeType { get; }

        protected TextField NodeGuid { get; }


        public GraphNodeInspector()
        {
            NodeType = new TextField("Type");
            NodeType.labelElement.style.minWidth = StyleKeyword.Auto;
            NodeType.labelElement.style.maxWidth = StyleKeyword.Auto;
            NodeType.labelElement.style.width = FieldLabelWidth;
            NodeType.SetEnabled(false);
            Add(NodeType);

            NodeGuid = new TextField("Guid");
            NodeGuid.labelElement.style.minWidth = StyleKeyword.Auto;
            NodeGuid.labelElement.style.maxWidth = StyleKeyword.Auto;
            NodeGuid.labelElement.style.width = FieldLabelWidth;
            NodeGuid.SetEnabled(false);
            Add(NodeGuid);
        }


        public virtual void SetTargetNode(GraphNode node)
        {
            TargetNode = node;

            NodeType.labelElement.style.width = FieldLabelWidth;
            NodeType.SetValueWithoutNotify(TargetNode.GetType().Name);

            NodeGuid.labelElement.style.width = FieldLabelWidth;
            NodeGuid.SetValueWithoutNotify(TargetNode.Guid);
        }


        protected void RaiseParamChangedEvent()
        {
            OnParamChanged?.Invoke();
        }
    }
}
