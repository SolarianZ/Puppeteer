using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.NodeData;
using UnityEngine;
using UnityEngine.UIElements;
using GraphViewNode = UnityEditor.Experimental.GraphView.Node;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public abstract class PlayableNode : AnimationGraphNode
    {
        public string Guid { get; }

        public abstract AnimationGraphPort OutputPort { get; }

        public IReadOnlyList<PlayableNode> InputNodes => InternalInputNodes;

        protected readonly List<PlayableNode> InternalInputNodes = new List<PlayableNode>();


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
        }


        public abstract void PopulateView(AnimationNodeData nodeData);

        public abstract AnimationNodeData CloneNodeData();
    }
}
