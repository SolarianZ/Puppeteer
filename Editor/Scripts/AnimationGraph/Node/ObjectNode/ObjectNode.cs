using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class ObjectNode<TObject, TOutput> : AnimationGraphNode
        where TObject : Object
    {
        public TObject Object
        {
            get => (TObject)_objectField.value;
            private set => _objectField.value = value;
        }
        private readonly ObjectField _objectField = new ObjectField();


        protected Port OutputPort { get; }


        protected ObjectNode(ObjectNodeData nodeData, string inputTitle, Color outputColor)
            : base(nodeData)
        {
            // input
            var inputLabel = new Label(inputTitle)
            {
                style =
                {
                    marginLeft=4
                }
            };
            inputContainer.Add(inputLabel);
            _objectField.objectType = typeof(TObject);
            _objectField.RegisterValueChangedCallback(OnObjectChanged);
            inputContainer.Add(_objectField);

            // output
            var isRebuild = NodeData.Ports.Count > 0;
            var outputPortGuid = NodeData.Ports.Count > 0 ? NodeData.Ports[0].Guid : null;
            var outputPort = InstantiatePort(Direction.Output, typeof(TOutput), outputPortGuid);
            outputPort.portColor = outputColor;
            outputContainer.Add(outputPort);
            var outputPortData = (PortData)outputPort.userData;
            Ports.Add(outputPortData.Guid, outputPort);
            if (!isRebuild)
            {
                NodeData.Ports.Add(outputPortData);
            }

            RefreshExpandedState();
            RefreshPorts();
        }


        public override void RebuildPorts()
        {
            var objectNodeData = (ObjectNodeData)NodeData;
            Object = objectNodeData.Object as TObject; // may be null
        }


        private void OnObjectChanged(ChangeEvent<Object> evt)
        {
            var objectNodeData = (ObjectNodeData)NodeData;
            objectNodeData.Object = evt.newValue;
        }
    }
}