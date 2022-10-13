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
        private readonly List<ParamInfo> _paramTable = new List<ParamInfo>();

        private ListView _paramListView;


        private void CreateBlackboardPanel()
        {
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
            _layoutContainer.LeftPane.Add(titleBar);

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
                fixedItemHeight = 24,
                makeItem = CreateNewParamListItem,
                bindItem = BindAssetListItem,
                itemsSource = _paramTable,
                selectionType = SelectionType.None,
            };
            _paramListView.itemIndexChanged += OnParamIndexChanged;
            _layoutContainer.LeftPane.Add(_paramListView);
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
                _paramTable.Add(new ParamInfo($"Float_{GenerateUniqueSuffix()}", ParamType.Float));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Integer", false, () =>
            {
                _paramTable.Add(new ParamInfo($"Integer_{GenerateUniqueSuffix()}", ParamType.Int));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Bool", false, () =>
            {
                _paramTable.Add(new ParamInfo($"Bool_{GenerateUniqueSuffix()}", ParamType.Bool));
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

            menu.DropDown(new Rect(menuPos, Vector2.zero), _layoutContainer.LeftPane);
        }

        private VisualElement CreateNewParamListItem()
        {
            return new ParamElement
            {
                style =
                {
                    flexGrow = 1,
                }
            };
        }

        private void BindAssetListItem(VisualElement listItem, int index)
        {
            var paramElem = (ParamElement)listItem;
            var paramInfo = _paramTable[index];
            paramElem.PopulateView(paramInfo);
            paramElem.OnParamChanged += OnParamChanged;
        }

        private void OnParamIndexChanged(int from, int to)
        {
            hasUnsavedChanges = true;
        }

        private void OnParamChanged(ParamElement paramElement)
        {
            hasUnsavedChanges = true;
        }
    }
}
