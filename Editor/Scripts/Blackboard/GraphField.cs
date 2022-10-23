using System;
using GBG.AnimationGraph.Graph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Blackboard
{
    public class GraphField : VisualElement
    {
        // public const string PARAM_NAME_MATCH_REGEX = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public event Action<Graph.Graph> OnWantsToRenameGraph;

        public event Action<Graph.Graph> OnWantsToOpenGraph;

        public event Action<Graph.Graph> OnWantsToDeleteGraph;


        private readonly Label _nameLabel;

        private readonly Label _typeLabel;

        private Graph.Graph _graph;

        private bool _deletable = true;


        public GraphField()
        {
            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;

            RegisterCallback<MouseDownEvent>(OnMouseClicked);

            // Name
            _nameLabel = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(_nameLabel);

            // Type
            _typeLabel = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            Add(_typeLabel);
        }

        public void SetGraphData(Graph.Graph graph, bool deletable)
        {
            _graph = graph;
            _deletable = deletable;

            // Name
            _nameLabel.text = _graph.Name;

            // Type
            _typeLabel.text = _graph.GraphType.ToString().Substring(0, 1);
        }


        private void OnMouseClicked(MouseDownEvent evt)
        {
            // Left mouse button double click to open graph
            if (evt.button == 0 && evt.clickCount > 1)
            {
                OnWantsToOpenGraph?.Invoke(_graph);
                return;
            }

            // Right mouse button click to show contextual menu
            if (evt.button == 1)
            {
                var menuPos = evt.mousePosition;
                var menu = new GenericDropdownMenu();

                // Rename
                menu.AddItem("Rename", false, () => { OnWantsToRenameGraph?.Invoke(_graph); });

                // Delete
                if (_deletable) menu.AddItem("Delete", false, () => { OnWantsToDeleteGraph?.Invoke(_graph); });
                else menu.AddDisabledItem("Delete", false);

                menu.DropDown(new Rect(menuPos, Vector2.zero), this);
            }
        }
    }
}
