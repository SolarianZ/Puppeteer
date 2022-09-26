using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class GraphElementInspector<TTarget> : VisualElement, IInspector<TTarget>
        where TTarget : GraphElement, IInspectable<TTarget>
    {
        public event Action OnParamChanged;


        protected Length FieldLabelWidth { get; set; } = Length.Percent(35);

        protected IInspectable<TTarget> Target { get; private set; }

        protected TextField NodeType { get; }

        protected TextField NodeGuid { get; }


        public GraphElementInspector()
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


        public virtual void SetTarget(TTarget target)
        {
            Target = target;

            NodeType.labelElement.style.width = FieldLabelWidth;
            NodeType.SetValueWithoutNotify(Target.GetType().Name);

            NodeGuid.labelElement.style.width = FieldLabelWidth;
            NodeGuid.SetValueWithoutNotify(Target.Guid);
        }


        protected void RaiseParamChangedEvent()
        {
            OnParamChanged?.Invoke();
        }
    }
}
