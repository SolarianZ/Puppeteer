using System;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class AnimationClipNode : PlayableNode
    {
        public sealed override AnimationGraphPort OutputPort { get; }

        private readonly ObjectField _clipField;

        // TODO: Explicit time

        private string _nodeTitle;


        public AnimationClipNode(string guid) : base(guid)
        {
            _clipField = new ObjectField
            {
                label = "Clip",
                objectType = typeof(AnimationClip)
            };
            _clipField.labelElement.style.minWidth = 0;
            _clipField.RegisterValueChangedCallback(OnClipChanged);
            inputContainer.Add(_clipField);

            OutputPort = InstantiatePort(Direction.Output, typeof(Playable));
            OutputPort.portName = "Output Pose";
            OutputPort.portColor = ColorTool.GetColor(typeof(Playable));
            outputContainer.Add(OutputPort);
        }

        private void OnClipChanged(ChangeEvent<UObject> _)
        {
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var clipNodeData = (AnimationClipNodeData)nodeData;
            _clipField.value = clipNodeData.AnimationClip;

            _nodeTitle = nodeData.EditorName;
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }
        }

        public override void OnInputConnected(AnimationGraphEdge edge)
        {
            throw new InvalidOperationException("Animation clip node can not have any input.");
        }

        public override void OnInputDisconnected(AnimationGraphEdge edge)
        {
            throw new InvalidOperationException("Animation clip node can not have any input.");
        }

        public override AnimationNodeData CloneNodeData()
        {
            // use '_nodeTitle', not 'title'
            throw new System.NotImplementedException();
        }
    }
}
