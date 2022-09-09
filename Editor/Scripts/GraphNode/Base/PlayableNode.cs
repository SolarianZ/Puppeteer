using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public abstract class PlayableNode : AnimationGraphNode
    {
        #region Port

        public AnimationGraphPort OutputPort { get; }


        public IReadOnlyList<MixerInput> MixerInputs => InternalMixerInputs;

        protected List<MixerInput> InternalMixerInputs { get; } = new List<MixerInput>();

        #endregion


        #region Data

        protected IReadOnlyList<ParamInfo> ParamTable { get; }

        protected abstract ParamField<float> PlaybackSpeedField { get; }

        public event Action<PlayableNode> OnNodeDataChanged;

        protected void RaiseNodeDataChangedEvent()
        {
            OnNodeDataChanged?.Invoke(this);
        }

        #endregion


        protected PlayableNode(string guid, List<ParamInfo> paramTable) : base(guid)
        {
            ParamTable = paramTable;

            // Title contents container
            var titleLabelContainer = new VisualElement()
            {
                name = "title-label-container",
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };
            titleContainer.Insert(0, titleLabelContainer);
            // Node title label
            var titleLabel = titleContainer.Q<Label>("title-label");
            titleLabel.style.flexGrow = 1;
            titleLabel.style.fontSize = 13;
            titleLabel.style.marginTop = 2;
            titleLabel.style.marginBottom = 0;
            titleLabelContainer.Add(titleLabel);
            // Node type label
            var nodeTypeLabel = new Label
            {
                name = "node-type-label",
                text = GetType().Name,
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Italic,
                    fontSize = 10,
                    marginLeft = 6,
                    marginTop = 2,
                    marginBottom = 2
                }
            };
            titleLabelContainer.Add(nodeTypeLabel);

            // Output
            OutputPort = InstantiatePort(Direction.Output, typeof(Playable));
            OutputPort.portName = "Output Pose";
            OutputPort.portColor = ColorTool.GetColor(typeof(Playable));
            outputContainer.Add(OutputPort);
        }


        public abstract void PopulateView(AnimationNodeData nodeData);


        #region Deep Clone

        public AnimationNodeData CloneNodeData()
        {
            var clone = CreateCloneNodeDataInstance();
            CloneNodeDataMembers(clone);

            return clone;
        }

        protected abstract AnimationNodeData CreateCloneNodeDataInstance();

        protected virtual void CloneNodeDataMembers(AnimationNodeData clone)
        {
            clone.EditorName = NodeName;

            clone.EditorPosition = GetPosition().position;

            clone.Guid = Guid;

            if (!PlaybackSpeedField.GetParamInfo(out var playbackSpeedParam))
            {
                Debug.LogError($"[Puppeteer::PlayableNode] Invalid 'PlaybackSpeed' param link on node '{title}'.");
            }

            clone.PlaybackSpeed = new ParamNameOrValue(playbackSpeedParam);
        }

        #endregion
    }
}
