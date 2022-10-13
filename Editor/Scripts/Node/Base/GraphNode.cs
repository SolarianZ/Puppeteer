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
    public class GraphNodeClickManipulator : MouseManipulator
    {
        private readonly Action<MouseDownEvent> _onClicked;


        public GraphNodeClickManipulator(Action<MouseDownEvent> onClicked)
        {
            _onClicked = onClicked;

            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }


        private void OnMouseDown(MouseDownEvent e)
        {
            _onClicked(e);
        }
    }

    public abstract partial class GraphNode : UNode, IInspectable<GraphNode>
    {
        public abstract string Guid { get; }

        internal AnimationGraphAsset GraphAsset { get; }


        /// <summary>
        /// By default this visual element will serve as a divider under the title container.
        /// </summary>
        public VisualElement BannerContainer { get; }

        public VisualElement IconContainer { get; }


        protected UGraphView GraphView => GetFirstAncestorOfType<UGraphView>();


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
            this.AddManipulator(new GraphNodeClickManipulator(OnClicked));
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


        #region Inspector

        public virtual IInspector<GraphNode> GetInspector()
        {
            var defaultInspector = new GraphElementInspector<GraphNode>();
            defaultInspector.SetTarget(this);

            return defaultInspector;
        }

        #endregion


        #region Events

        public event Action OnNodeChanged;

        /// <summary>
        /// Don't raise events which is contained in GraphView.graphViewChanged callback.
        /// </summary>
        protected void RaiseNodeChangedEvent()
        {
            OnNodeChanged?.Invoke();
        }


        public event Action<GraphNode> OnDoubleClicked;

        private void OnClicked(MouseDownEvent evt)
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
