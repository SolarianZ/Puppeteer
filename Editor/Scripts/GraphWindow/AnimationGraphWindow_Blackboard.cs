using System;
using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private const float _BLACKBOARD_PANEL_WIDTH = 300;

        private readonly List<ParamInfo> _paramInfos = new List<ParamInfo>();

        private VisualElement _blackboardPanel;

        private ListView _paramListView;


        private void CreateBlackboardPanel()
        {
            // Panel
            _blackboardPanel = new VisualElement
            {
                name = "blackboard-panel",
                style =
                {
                    width = _BLACKBOARD_PANEL_WIDTH,
                    height = Length.Percent(100),
                    borderRightWidth = 1,
                    borderRightColor = Color.black,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                }
            };
            _layoutContainer.Add(_blackboardPanel);

            // Title bar
            var titleBar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.black,
                }
            };
            _blackboardPanel.Add(titleBar);

            // Parameter label
            var paramLabel = new Label("Parameters")
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = 4,
                }
            };
            titleBar.Add(paramLabel);

            // Add new param
            var addParamButton = new Button()
            {
                text = "+"
            };
            if (addParamButton.clickable == null)
            {
                addParamButton.clickable = new Clickable((Action)null);
            }

            addParamButton.clickable.clickedWithEventInfo += OnAddParamButtonClicked;
            titleBar.Add(addParamButton);

            // Parameter list view
            _paramListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                makeItem = CreateNewParamListItem,
                bindItem = BindAssetListItem,
                itemsSource = _paramInfos,
                selectionType = SelectionType.None,
            };
            _blackboardPanel.Add(_paramListView);
        }

        private void OnAddParamButtonClicked(EventBase evt)
        {
            static string GenerateUniqueSuffix()
            {
                return Mathf.Abs(GUID.Generate().GetHashCode()).ToString();
            }

            var menu = new GenericDropdownMenu();
            menu.AddItem("Float", false, () =>
            {
                _paramInfos.Add(new ParamInfo($"Float_{GenerateUniqueSuffix()}", ParamType.Float));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Integer", false, () =>
            {
                _paramInfos.Add(new ParamInfo($"Integer_{GenerateUniqueSuffix()}", ParamType.Int));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Bool", false, () =>
            {
                _paramInfos.Add(new ParamInfo($"Bool_{GenerateUniqueSuffix()}", ParamType.Bool));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });

            var menuPos = Vector2.zero;
            if (evt is IMouseEvent)
            {
                menuPos = ((IMouseEvent)evt).mousePosition;
            }
            else if (evt is IPointerEvent)
            {
                menuPos = ((IPointerEvent)evt).position;
            }
            else if (evt.target is VisualElement)
            {
                menuPos = ((VisualElement)evt.target).layout.center;
            }

            menu.DropDown(new Rect(menuPos, Vector2.zero), _blackboardPanel);
        }

        private VisualElement CreateNewParamListItem()
        {
            return new ParamElement();
        }

        private void BindAssetListItem(VisualElement listItem, int index)
        {
            var paramElem = (ParamElement)listItem;
            var paramInfo = _paramInfos[index];
            paramElem.PopulateView(paramInfo);
            paramElem.OnParamChanged += OnParamChanged;
        }

        private void OnParamChanged(ParamElement paramElement)
        {
            hasUnsavedChanges = true;
        }
    }
}
