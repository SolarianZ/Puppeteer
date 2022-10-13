using System;
using System.Linq;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class MixerInput : VisualElement
    {
        public AnimationGraphPort InputPort { get; }

        // TODO: Restrict available parameters by date type
        public ParamField<float> InputWeightField { get; }


        private readonly Button _deleteSelfButton;


        public MixerInput(Action<MixerInput> deleter, Length? valueLabelWidth = null)
        {
            style.width = Length.Percent(100);

            // Delete button
            _deleteSelfButton = new Button(() => deleter(this))
            {
                name = "delete-button",
                text = "-",
                tooltip = "Delete",
                style =
                {
                    width = 12,
                    height = 12,
                    position = Position.Absolute,
                    top = 2,
                    right = 2,
                }
            };
            Add(_deleteSelfButton);

            // Input port
            InputPort = AnimationGraphPort.Create(Direction.Input, typeof(Playable));
            InputPort.portName = "Input Pose";
            InputPort.portColor = ColorTool.GetColor(typeof(Playable));
            Add(InputPort);

            // Weight param
            InputWeightField = new ParamField<float>("Input Weight", valueLabelWidth);
            Add(InputWeightField);
        }

        public void UpdateIndex(int index, bool deletable)
        {
            InputPort.portName = $"Input Pose {index.ToString()}";
            InputWeightField.SetLabel($"Input Weight {index.ToString()}");

            _deleteSelfButton.SetEnabled(deletable);
        }

        public void Disconnect()
        {
            foreach (var edge in InputPort.connections.ToArray())
            {
                edge.input?.Disconnect(edge);
                edge.output?.Disconnect(edge);
                edge.RemoveFromHierarchy();
            }
        }

        public virtual MixerInputInfo GetInputInfo()
        {
            var inputNode = InputPort.ConnectedNode;
            if (inputNode == null)
            {
                return null;
            }

            InputWeightField.GetParamInfo(out var inputWeightParam);
            return new MixerInputInfo(inputNode.Guid, new ParamNameOrValue(inputWeightParam));
        }
    }

    public class LayerMixerInput : MixerInput
    {
        public Toggle IsAdditive { get; }

        public ObjectField AvatarMask { get; }


        public LayerMixerInput(Action<MixerInput> deleter, Length? valueLabelWidth = null)
            : base(deleter, valueLabelWidth)
        {
            // Is additive
            IsAdditive = new Toggle("Is Additive")
            {
                value = false
            };
            Add(IsAdditive);

            // Avatar mask
            AvatarMask = new ObjectField("Avatar Mask")
            {
                objectType = typeof(AvatarMask)
            };
            Add(AvatarMask);

            // Label width
            if (valueLabelWidth != null)
            {
                var labelWidth = valueLabelWidth.Value;

                // Additive label
                var additiveLabelStyle = IsAdditive.Q<Label>().style;
                additiveLabelStyle.minWidth = labelWidth;
                additiveLabelStyle.width = labelWidth;
                additiveLabelStyle.marginLeft = 20;
                additiveLabelStyle.marginRight = 8;

                // Avatar mask label
                var avatarMaskLabelStyle = AvatarMask.Q<Label>().style;
                avatarMaskLabelStyle.minWidth = labelWidth;
                avatarMaskLabelStyle.width = labelWidth;
                avatarMaskLabelStyle.marginLeft = 20;
                avatarMaskLabelStyle.marginRight = 8;
            }
        }

        public override MixerInputInfo GetInputInfo()
        {
            var inputNode = InputPort.ConnectedNode;
            if (inputNode == null)
            {
                return null;
            }

            InputWeightField.GetParamInfo(out var inputWeightParam);
            return new LayerMixerInputInfo(inputNode.Guid, new ParamNameOrValue(inputWeightParam),
                IsAdditive.value, (AvatarMask)AvatarMask.value);
        }
    }
}