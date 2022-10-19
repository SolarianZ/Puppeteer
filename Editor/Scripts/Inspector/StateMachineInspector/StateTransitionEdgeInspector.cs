using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Editor.GraphEdge;
using GBG.AnimationGraph.Editor.GraphEditor;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class ConditionDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 64;

        public event Action<TransitionCondition> OnConditionChanged;


        private readonly List<ParamInfo> _paramTable;

        private readonly ParamField _leftParam;

        private readonly OperatorDrawer _operatorDrawer;

        private readonly ParamField _rightParam;

        private TransitionCondition _condition;


        public ConditionDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
        {
            _paramTable = paramTable;

            _leftParam = new ParamField(nameLabelWidth);
            _leftParam.OnParamChanged += _ => OnConditionChanged?.Invoke(_condition);
            Add(_leftParam);

            _operatorDrawer = new OperatorDrawer(nameLabelWidth);
            _operatorDrawer.Button.clickable.clickedWithEventInfo += ShowOperatorPopupList;
            Add(_operatorDrawer);

            _rightParam = new ParamField(nameLabelWidth);
            _leftParam.OnParamChanged += _ => OnConditionChanged?.Invoke(_condition);
            Add(_rightParam);
        }


        // TODO: Limit right param type & validate conditions
        public void SetCondition(TransitionCondition condition)
        {
            _condition = condition;

            _leftParam.SetParamTarget("Left Operand", _condition.LeftParam, _condition.ParamType,
                _paramTable, ParamLinkState.LinkedLocked, ParamActiveState.ActiveLocked, null);

            _operatorDrawer.Button.text = _condition.Operator.ToString();

            _rightParam.SetParamTarget("Right Operand", _condition.RightParam,
                _condition.ParamType, _paramTable,
                _condition.RightParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, null);
        }


        private void ShowOperatorPopupList(EventBase evt)
        {
            var menu = new GenericDropdownMenu();

            foreach (var opObj in Enum.GetValues(typeof(ConditionOperator)))
            {
                var op = (ConditionOperator)opObj;
                if (_condition.ParamType == ParamType.Bool)
                {
                    switch (op)
                    {
                        case ConditionOperator.Equals:
                        case ConditionOperator.NotEquals:
                            menu.AddItem(op.ToString(), op == _condition.Operator, ChangeOperator, op);
                            break;

                        default:
                            menu.AddDisabledItem(op.ToString(), false);
                            break;
                    }
                }
                else
                {
                    menu.AddItem(op.ToString(), op == _condition.Operator, ChangeOperator, op);
                }
            }

            menu.DropDown(new Rect(evt.originalMousePosition, Vector2.zero), _operatorDrawer.Button);
        }

        private void ChangeOperator(object opObj)
        {
            _condition.Operator = (ConditionOperator)opObj;
            _operatorDrawer.Button.text = _condition.Operator.ToString();

            OnConditionChanged?.Invoke(_condition);
        }


        private class OperatorDrawer : VisualElement
        {
            public Button Button { get; }

            public OperatorDrawer(Length nameLabelWidth)
            {
                style.flexDirection = FlexDirection.Row;
                style.flexShrink = 0;
                style.marginLeft = 3;
                style.marginRight = 3;

                var label = new Label
                {
                    text = "Operator",
                    style =
                    {
                        width = nameLabelWidth,
                        marginRight = 2,
                        overflow = Overflow.Hidden,
                        unityTextAlign = TextAnchor.MiddleLeft,
                    }
                };
                Add(label);

                Button = new Button
                {
                    text = ConditionOperator.Equals.ToString(),
                    style =
                    {
                        flexGrow = 1,
                        marginLeft = 0,
                        marginRight = 0,
                    }
                };
                Add(Button);
            }
        }
    }

    public delegate void WantsToIndicateTransition(StateGraphNode fromNode, StateGraphNode destNode);

    public delegate void WantsToDeleteTransition(StateGraphNode fromNode, StateGraphNode destNode);

    public sealed class TransitionDrawer : VisualElement
    {
        private readonly Action _onParamChanged;

        private readonly Length _fieldLabelWidth;

        private readonly Foldout _foldout;

        private readonly TextField _destNodeGuid;

        private readonly Slider _exitTime;

        private readonly FloatField _fadeTime;

        private readonly EnumField _transitionMode;

        private readonly CurveField _blendCurve;

        private readonly ListView _conditionListView;

        private readonly Action<int> _addTransitionElement;

        private readonly Action<int> _removeTransitionElement;

        private readonly List<ParamInfo> _paramTable;

        private StateGraphNode _fromNode;

        private StateGraphNode _destNode;

        private Transition _transition;


        public TransitionDrawer(List<ParamInfo> paramTable, Length fieldLabelWidth,
            Action onParamChanged,
            Action<int> addTransitionElement,
            Action<int> removeTransitionElement,
            WantsToIndicateTransition onWantsToIndicate,
            WantsToDeleteTransition onWantsToDelete)
        {
            _onParamChanged = onParamChanged;
            _fieldLabelWidth = fieldLabelWidth;
            _paramTable = paramTable;
            _addTransitionElement = addTransitionElement;
            _removeTransitionElement = removeTransitionElement;

            _foldout = new Foldout
            {
                value = true,
            };
            Add(_foldout);

            // Dest node guid
            _destNodeGuid = new TextField("Dest Node Guid");
            _destNodeGuid.labelElement.style.minWidth = StyleKeyword.Auto;
            _destNodeGuid.labelElement.style.maxWidth = StyleKeyword.Auto;
            _destNodeGuid.labelElement.style.width = _fieldLabelWidth;
            _destNodeGuid.SetEnabled(false);
            _foldout.contentContainer.Add(_destNodeGuid);

            // Exit time
            _exitTime = new Slider("Exit Time(%)", 0f, 1f)
            {
                showInputField = true,
            };
            _exitTime.labelElement.style.minWidth = StyleKeyword.Auto;
            _exitTime.labelElement.style.maxWidth = StyleKeyword.Auto;
            _exitTime.labelElement.style.width = _fieldLabelWidth;
            _exitTime.RegisterValueChangedCallback(OnExitTimeChanged);
            _foldout.contentContainer.Add(_exitTime);

            // Fade time
            _fadeTime = new FloatField("Fade Time(s)");
            _fadeTime.labelElement.style.minWidth = StyleKeyword.Auto;
            _fadeTime.labelElement.style.maxWidth = StyleKeyword.Auto;
            _fadeTime.labelElement.style.width = _fieldLabelWidth;
            _fadeTime.RegisterValueChangedCallback(OnFadeTimeChanged);
            _foldout.contentContainer.Add(_fadeTime);

            // Transition mode
            _transitionMode = new EnumField("Transition Mode", TransitionMode.Smooth);
            _transitionMode.labelElement.style.minWidth = StyleKeyword.Auto;
            _transitionMode.labelElement.style.maxWidth = StyleKeyword.Auto;
            _transitionMode.labelElement.style.width = _fieldLabelWidth;
            _transitionMode.RegisterValueChangedCallback(OnTransitionModeChanged);
            _foldout.contentContainer.Add(_transitionMode);

            // Blend curve
            _blendCurve = new CurveField("BlendCurve");
            _blendCurve.labelElement.style.minWidth = StyleKeyword.Auto;
            _blendCurve.labelElement.style.maxWidth = StyleKeyword.Auto;
            _blendCurve.labelElement.style.width = _fieldLabelWidth;
            _blendCurve.RegisterValueChangedCallback(OnBlendCurveChanged);
            _foldout.contentContainer.Add(_blendCurve);

            // Conditions
            var conditionListViewLabel = new Label("Conditions")
            {
                style =
                {
                    height = 20,
                    marginLeft = 3,
                    marginRight = 3,
                    unityTextAlign = TextAnchor.MiddleLeft,
                }
            };
            _foldout.contentContainer.Add(conditionListViewLabel);
            _conditionListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = ConditionDrawer.DRAWER_HEIGHT,
                makeItem = MakeConditionListItem,
                bindItem = BindConditionListItem,
                selectionType = SelectionType.Single,
                showAddRemoveFooter = true,
            };
            _conditionListView.itemsAdded += OnConditionItemAdded;
            _conditionListView.itemsRemoved += OnConditionItemRemoved;
            _foldout.contentContainer.Add(_conditionListView);

            // Buttons
            var buttonContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginBottom = 20,
                },
            };
            _foldout.Add(buttonContainer);

            // Indicate button
            var indicateButton = new Button(() => { onWantsToIndicate(_fromNode, _destNode); })
            {
                text = "Indicate",
                style =
                {
                    width = 64,
                },
            };
            buttonContainer.Add(indicateButton);

            // Delete button
            var deleteButton = new Button(() => { onWantsToDelete(_fromNode, _destNode); })
            {
                text = "Delete Transition",
            };
            buttonContainer.Add(deleteButton);
        }

        public void SetTransition(StateGraphNode fromNode, StateGraphNode destNode, Transition transition)
        {
            _fromNode = fromNode;
            _destNode = destNode;
            _transition = transition;

            _foldout.text = $"{fromNode.StateName} -> {destNode.StateName}";

            _destNodeGuid.SetValueWithoutNotify(_transition.DestStateGuid);

            _exitTime.SetValueWithoutNotify(_transition.ExitTime);

            _fadeTime.SetValueWithoutNotify(_transition.FadeTime);

            _transitionMode.SetValueWithoutNotify(_transition.TransitionMode);
            _transitionMode.MarkDirtyRepaint();

            _blendCurve.SetValueWithoutNotify(_transition.BlendCurve);

            _conditionListView.itemsSource = _transition.Conditions;
            _conditionListView.RefreshItems();
        }


        private void OnExitTimeChanged(ChangeEvent<float> evt)
        {
            _transition.ExitTime = _exitTime.value;
            _onParamChanged();
        }

        private void OnFadeTimeChanged(ChangeEvent<float> evt)
        {
            if (evt.newValue < 0)
            {
                _fadeTime.SetValueWithoutNotify(0);
            }

            _transition.FadeTime = _fadeTime.value;

            _onParamChanged();
        }

        private void OnTransitionModeChanged(ChangeEvent<Enum> evt)
        {
            _transition.TransitionMode = (TransitionMode)_transitionMode.value;
            _onParamChanged();
        }

        private void OnBlendCurveChanged(ChangeEvent<AnimationCurve> evt)
        {
            _transition.BlendCurve = _blendCurve.value;
            _onParamChanged();
        }

        private VisualElement MakeConditionListItem()
        {
            var mixerDrawer = new ConditionDrawer(_paramTable, _fieldLabelWidth);
            mixerDrawer.OnConditionChanged += OnConditionChanged;

            return mixerDrawer;
        }

        private void BindConditionListItem(VisualElement element, int index)
        {
            var drawer = (ConditionDrawer)element;
            drawer.SetCondition(_transition.Conditions[index]);
        }

        private void OnConditionItemAdded(IEnumerable<int> indices)
        {
            var index = indices.First();
            _transition.Conditions[index] = new TransitionCondition();
            _addTransitionElement(index);
            _onParamChanged();
        }

        private void OnConditionItemRemoved(IEnumerable<int> indices)
        {
            _removeTransitionElement(indices.First());
            _onParamChanged();
        }

        private void OnConditionChanged(TransitionCondition condition)
        {
            _onParamChanged();
        }
    }

    public sealed class StateTransitionEdgeInspector : GraphElementInspector<StateTransitionEdge>
    {
        private new StateTransitionEdge Target => (StateTransitionEdge)base.Target;

        private readonly TransitionDrawer _transition0;

        private readonly TransitionDrawer _transition1;

        private readonly WantsToIndicateTransition _onWantsToIndicateTransition;

        private readonly WantsToDeleteTransition _onWantsToDeleteTransition;


        public StateTransitionEdgeInspector(List<ParamInfo> paramTable, Action<int> addTransitionElement,
            Action<int> removeTransitionElement, WantsToIndicateTransition onWantsToIndicateTransition,
            WantsToDeleteTransition onWantsToDeleteTransition)
        {
            _onWantsToIndicateTransition = onWantsToIndicateTransition;
            _onWantsToDeleteTransition = onWantsToDeleteTransition;

            var scrollView = new ScrollView();
            Add(scrollView);

            _transition0 = new TransitionDrawer(paramTable, FieldLabelWidth, OnTransitionParamChanged,
                addTransitionElement, removeTransitionElement, OnWantsToIndicateTransition, OnWantsToDeleteTransition);
            scrollView.contentContainer.Add(_transition0);

            _transition1 = new TransitionDrawer(paramTable, FieldLabelWidth, OnTransitionParamChanged,
                addTransitionElement, removeTransitionElement, OnWantsToIndicateTransition, OnWantsToDeleteTransition);
            scrollView.contentContainer.Add(_transition1);
        }

        public override void SetTarget(StateTransitionEdge target)
        {
            base.SetTarget(target);

            if (Target == null || Target.IsEntryEdge)
            {
                // Entry node
                _transition0.visible = false;
                _transition0.SetEnabled(false);
                _transition1.visible = false;
                _transition1.SetEnabled(false);
                return;
            }

            var transition0 = Target.ConnectedNode0.OutputTransitions.FirstOrDefault(e =>
                e.IsConnection(Target.ConnectedNode0, Target.ConnectedNode1));
            if (transition0 != null)
            {
                _transition0.visible = true;
                _transition0.SetEnabled(true);
                _transition0.style.position = Position.Relative;
                var data0 = target.ConnectedNode0.Transitions.Find(t =>
                    t.DestStateGuid.Equals(target.ConnectedNode1.Guid));
                _transition0.SetTransition(transition0.ConnectedNode0, transition0.ConnectedNode1, data0);
            }
            else
            {
                _transition0.visible = false;
                _transition0.SetEnabled(false);
                _transition0.style.position = Position.Absolute;
            }

            var transition1 = Target.ConnectedNode1.OutputTransitions.FirstOrDefault(e =>
                e.IsConnection(Target.ConnectedNode0, Target.ConnectedNode1));
            if (transition1 != null)
            {
                _transition1.visible = true;
                _transition1.SetEnabled(true);
                _transition1.style.position = Position.Relative;
                var data1 = target.ConnectedNode1.Transitions.Find(t =>
                    t.DestStateGuid.Equals(target.ConnectedNode0.Guid));
                _transition1.SetTransition(transition1.ConnectedNode1, transition1.ConnectedNode0, data1);
            }
            else
            {
                _transition1.visible = false;
                _transition1.SetEnabled(false);
                _transition1.style.position = Position.Absolute;
            }
        }


        private void OnWantsToIndicateTransition(StateGraphNode fromNode, StateGraphNode destNode)
        {
            _onWantsToIndicateTransition(fromNode, destNode);
        }

        private void OnWantsToDeleteTransition(StateGraphNode fromNode, StateGraphNode destNode)
        {
            _onWantsToDeleteTransition(fromNode, destNode);

            SetTarget(Target.IsConnection(fromNode, destNode) ? Target : null);
            RaiseDataChangedEvent(DataCategories.TransitionData);
        }

        private void OnTransitionParamChanged()
        {
            RaiseDataChangedEvent(DataCategories.TransitionData);
        }
    }
}
