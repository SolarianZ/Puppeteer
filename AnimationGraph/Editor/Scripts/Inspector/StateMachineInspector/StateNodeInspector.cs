using GBG.AnimationGraph.Editor.Node;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class StateNodeInspector : GraphElementInspector<GraphNode>
    {
        protected new StateNode Target => (StateNode)base.Target;

        protected TextField StateName { get; }

        private readonly Foldout _destFoldout;


        public StateNodeInspector()
        {
            // State name
            StateName = new TextField("Name");
            StateName.labelElement.style.minWidth = StyleKeyword.Auto;
            StateName.labelElement.style.maxWidth = StyleKeyword.Auto;
            StateName.labelElement.style.width = FieldLabelWidth;
            StateName.RegisterValueChangedCallback(OnStateNameChanged);
            Add(StateName);

            // Dest transitions
            _destFoldout = new Foldout
            {
                text = "Dest Transitions",
                value = true,
            };
            Add(_destFoldout);
        }

        public override void SetTarget(GraphNode target)
        {
            base.SetTarget(target);

            StateName.SetValueWithoutNotify(Target.StateName);

            _destFoldout.contentContainer.Clear();
            for (var i = 0; i < Target.OutputTransitions.Count; i++)
            {
                var destTransition = Target.OutputTransitions[i];
                destTransition.TryGetConnectedNode(Target, out var destNode);
                _destFoldout.Add(new DestDrawer(destNode));

                if (i != Target.OutputTransitions.Count - 1)
                {
                    var separator = new VisualElement
                    {
                        name = "separator",
                        style =
                        {
                            flexGrow = 1,
                            height = 1,
                            backgroundColor = new Color(35 / 255f, 35 / 255f, 35 / 255f),
                        },
                    };
                    _destFoldout.Add(separator);
                }
            }
        }


        private void OnStateNameChanged(ChangeEvent<string> evt)
        {
            Target.StateName = evt.newValue;
            RaiseParamChangedEvent();
        }


        private class DestDrawer : VisualElement
        {
            public DestDrawer(StateNode destNode)
            {
                style.marginTop = 3;
                style.marginBottom = 3;

                var nodeName = new TextField("Node Name")
                {
                    value = destNode?.StateName,
                };
                nodeName.SetEnabled(false);
                Add(nodeName);

                var nodeGuid = new TextField("Node Guid")
                {
                    value = destNode?.Guid,
                };
                nodeGuid.SetEnabled(false);
                Add(nodeGuid);
            }
        }
    }
}
