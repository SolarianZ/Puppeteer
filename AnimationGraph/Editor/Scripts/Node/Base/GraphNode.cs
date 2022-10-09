using System;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.Inspector;
using GBG.AnimationGraph.Editor.Port;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UGraphView = UnityEditor.Experimental.GraphView.GraphView;
using UNode = UnityEditor.Experimental.GraphView.Node;
using UEdge = UnityEditor.Experimental.GraphView.Edge;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor.Node
{
    public abstract partial class GraphNode : UNode, IInspectable<GraphNode>
    {
        public abstract string Guid { get; }

        internal AnimationGraphAsset GraphAsset { get; }


        /// <summary>
        /// By default this visual element will serve as a divider under the title container.
        /// </summary>
        public VisualElement BannerContainer { get; }

        public VisualElement IconContainer { get; }

        public event Action OnNodeChanged;

        public UGraphView GraphView
        {
            get
            {
                if (_graphView == null)
                {
                    _graphView = GetFirstAncestorOfType<UGraphView>();
                }

                return _graphView;
            }
        }

        private UGraphView _graphView;


        protected GraphNode(AnimationGraphAsset graphAsset)
        {
            GraphAsset = graphAsset;

            // Hide collapse button
            base.titleButtonContainer.Clear();
            var titleLabel = titleContainer.Q<Label>(name: "title-label");
            titleLabel.style.marginRight = 6;

            // Use title button container as icon container
            IconContainer = base.titleButtonContainer;

            // Banner container
            BannerContainer = mainContainer.Q("divider");

            // Callbacks
            RegisterCallback<ClickEvent>(OnClick);
        }


        public override UPort InstantiatePort(Orientation orientation,
            Direction direction, UPort.Capacity capacity, Type type)
        {
            var port = GraphPort.Create<FlowingGraphEdge>(direction, type);
            port.OnConnected += OnPortConnected;
            port.OnDisconnected += OnPortDisconnected;

            return port;
        }

        protected GraphPort InstantiatePort(Direction direction, Type type)
        {
            return (GraphPort)InstantiatePort(Orientation.Horizontal, direction, UPort.Capacity.Single, type);
        }

        protected virtual void OnPortConnected(UEdge edge)
        {
            RaiseNodeChangedEvent();
        }

        protected virtual void OnPortDisconnected(UEdge edge)
        {
            RaiseNodeChangedEvent();
        }


        /// <summary>
        /// Don't raise events if the event is contained in GraphView.graphViewChanged callback.
        /// </summary>
        protected void RaiseNodeChangedEvent()
        {
            OnNodeChanged?.Invoke();
        }


        #region Inspector

        public virtual IInspector<GraphNode> GetInspector()
        {
            var defaultInspector = new GraphElementInspector<GraphNode>();
            defaultInspector.SetTarget(this);

            return defaultInspector;
        }

        #endregion


        #region Events

        public event Action<GraphNode> OnDoubleClicked;


        private void OnClick(ClickEvent evt)
        {
            if (evt.clickCount == 2)
            {
                OnDoubleClicked?.Invoke(this);
            }
        }

        #endregion
    }

    // Api Masks
    public partial class GraphNode
    {
        // ReSharper disable once InconsistentNaming
        [Obsolete("Don't use!", true)]
        public new VisualElement titleButtonContainer { get; } = null;
    }
}
