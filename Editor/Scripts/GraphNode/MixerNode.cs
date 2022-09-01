using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class MixerInputElement : VisualElement
    {
        public AnimationGraphPort InputPort { get; }

        public ParamField<float> InputWeightField { get; }


        public MixerInputElement()
        {
            style.width = Length.Percent(100);

            // Input port
            InputPort = AnimationGraphPort.Create(Direction.Input, typeof(Playable));
            InputPort.portName = "Input Pose";
            InputPort.portColor = ColorTool.GetColor(typeof(Playable));
            Add(InputPort);

            // Weight param
            InputWeightField = new ParamField<float>("Input Weight");
            Add(InputWeightField);
        }

        public void UpdateIndex(int index)
        {
            InputPort.portName = $"Input Pose {index}";
            InputWeightField.SetLabel($"Input Weight {index}");
        }
    }

    public class MixerNode : PlayableNode
    {
        protected override ParamField<float> PlaybackSpeedField { get; }

        private MixerInputElement _inputField;


        public MixerNode(string guid) : base(guid)
        {
            // Test
            _inputField = new MixerInputElement();
            inputContainer.Add(_inputField);
        }

        public override void PopulateView(AnimationNodeData nodeData, List<ParamInfo> paramTable)
        {
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            throw new NotImplementedException();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);
        }

        #endregion
    }
}
