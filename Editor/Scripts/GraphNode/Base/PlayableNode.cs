using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor;
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


        public IReadOnlyList<AnimationGraphPort> InputPorts => InternalInputPorts;

        protected List<AnimationGraphPort> InternalInputPorts { get; } = new List<AnimationGraphPort>();

        public IReadOnlyList<PlayableNode> InputNodes => InternalInputNodes;

        protected List<PlayableNode> InternalInputNodes { get; } = new List<PlayableNode>();

        #endregion


        #region Data

        public string Guid { get; }

        protected virtual string NodeName
        {
            get => title;
            set => title = value;
        }

        protected abstract ParamField<float> PlaybackSpeedField { get; }


        public event Action<PlayableNode> OnNodeDataChanged;

        protected void RaiseNodeDataChangedEvent()
        {
            OnNodeDataChanged?.Invoke(this);
        }

        #endregion


        protected PlayableNode() : this(NewGuid())
        {
        }

        protected PlayableNode(string guid)
        {
            Guid = guid;

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


        public abstract void PopulateView(AnimationNodeData nodeData, List<ParamInfo> paramTable);


        #region GUID

        private static Func<string> _customGuidGenerator;


        public static void SetCustomGuidGenerator(Func<string> customGuidGenerator)
        {
            _customGuidGenerator = customGuidGenerator;
        }

        public static string NewGuid()
        {
            if (_customGuidGenerator != null)
            {
                return _customGuidGenerator();
            }

            return GUID.Generate().ToString();
        }

        #endregion


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
