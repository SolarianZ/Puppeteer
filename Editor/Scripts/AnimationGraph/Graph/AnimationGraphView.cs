using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UGraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public abstract class AnimationGraphView : UGraphView
    {
        public abstract AnimationGraphNode RootNode { get; }


        protected AnimationGraphView()
        {
            style.width = new Length(100, LengthUnit.Percent);
            style.height = new Length(100, LengthUnit.Percent);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        }


        private IEnumerable<Type> _nodeTypesCache;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (selection.Count == 0)
            {
                _nodeTypesCache ??= from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from type in assembly.GetTypes()
                                    where !type.IsAbstract && type.IsSubclassOf(typeof(AnimationGraphNode))
                                    select type;

                foreach (var nodeType in _nodeTypesCache)
                {
                    evt.menu.AppendAction($"Create {nodeType.Name}", (action) =>
                    {
                        var ctor = nodeType.GetConstructor(Array.Empty<Type>());
                        if (ctor == null)
                        {
                            Debug.LogError($"{nodeType.Name} does not have default constructor(without parameter).");
                            return;
                        }
                        var node = (AnimationGraphNode)ctor.Invoke(null);
                        node.SetPosition(new Rect(action.eventInfo.localMousePosition, Vector2.zero));
                        AddElement(node);
                    });
                }
            }
            else if (!selection.Contains(RootNode))
            {
                //evt.menu.AppendAction("Can not manipulate output node", null, DropdownMenuAction.Status.Disabled);
                base.BuildContextualMenu(evt);
            }
        }


        private readonly List<Port> _compatiblePortsCache = new List<Port>();

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //base.GetCompatiblePorts(startPort, nodeAdapter);

            _compatiblePortsCache.Clear();
            _compatiblePortsCache.AddRange(from port in ports
                                           where port.node != startPort.node &&
                                                 port.direction != startPort.direction &&
                                                 port.portType == startPort.portType
                                           select port);

            return _compatiblePortsCache;
        }
    }
}
