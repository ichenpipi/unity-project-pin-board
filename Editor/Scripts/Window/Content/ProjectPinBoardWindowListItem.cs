using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（内容列表条目）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region ListItem Definition

        /// <summary>
        /// 列表条目
        /// </summary>
        private class ListItem : VisualElement
        {

            /// <summary>
            /// 下标
            /// </summary>
            public int index = -1;

            /// <summary>
            /// 数据
            /// </summary>
            public new ItemInfo userData = null;

            /// <summary>
            /// 置顶图标
            /// </summary>
            public readonly Image topImage = null;

            /// <summary>
            /// 图标
            /// </summary>
            public readonly Image iconImage = null;

            /// <summary>
            /// 名称标签
            /// </summary>
            public readonly Label nameLabel = null;

            /// <summary>
            /// 定位按钮
            /// </summary>
            public readonly Button locateButton = null;

            /// <summary>
            /// 名称输入框
            /// </summary>
            public readonly TextField nameTextField = null;

            /// <summary>
            /// 显示名称附加后缀
            /// </summary>
            private const string k_DisplayNameSuffix = "^";

            public ListItem()
            {
                // 自身样式
                this.style.paddingTop = 0;
                this.style.paddingBottom = 0;
                this.style.paddingLeft = 0;
                this.style.paddingRight = 0;
                this.style.flexDirection = FlexDirection.Row;
                this.style.alignItems = Align.Center;

                // 容器
                VisualElement one = new VisualElement()
                {
                    name = "1",
                    pickingMode = PickingMode.Ignore,
                    style =
                    {
                        width = 15,
                        height = Length.Percent(100),
                        marginLeft = 0,
                        flexShrink = 0,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    },
                };
                this.Add(one);
                {
                    // 图标
                    topImage = new Image
                    {
                        name = "Top",
                        // image = ProjectPinBoardUtil.GetIcon("UpArrow"),
                        image = ProjectPinBoardUtil.GetIcon("Download-Available"),
                        pickingMode = PickingMode.Ignore,
                        scaleMode = ScaleMode.ScaleToFit,
                        style =
                        {
                            width = 12,
                        },
                    };
#if UNITY_2021_1_OR_NEWER
                    topImage.transform.rotation = Quaternion.Euler(0, 0, 180);
                    one.Add(topImage);
#else
                    // 兼容 Unity 2020
                    // 添加一个轴节点，用于实现图标基于中心点旋转180度的效果
                    VisualElement pivot = new VisualElement
                    {
                        name = "Pivot",
                        pickingMode = PickingMode.Ignore,
                        style =
                        {
                            width = 0,
                            height = 0,
                            flexGrow = 1,
                            top = Length.Percent(100),
                            left = Length.Percent(0),
                            alignItems = Align.Center,
                            justifyContent = Justify.Center,
                        },
                        transform =
                        {
                            rotation = Quaternion.Euler(0, 0, 180)
                        }
                    };
                    one.Add(pivot);
                    pivot.Add(topImage);
#endif
                }

                // 容器
                VisualElement two = new VisualElement()
                {
                    name = "2",
                    pickingMode = PickingMode.Ignore,
                    style =
                    {
                        width = 17,
                        height = Length.Percent(100),
                        marginLeft = 0,
                        flexShrink = 0,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    },
                };
                this.Add(two);
                {
                    // 图标
                    iconImage = new Image()
                    {
                        name = "Icon",
                        pickingMode = PickingMode.Ignore,
                        scaleMode = ScaleMode.ScaleToFit,
                        style = { },
                    };
                    two.Add(iconImage);
                }

                // 容器
                VisualElement three = new VisualElement()
                {
                    name = "3",
                    pickingMode = PickingMode.Ignore,
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        height = Length.Percent(100),
                        marginLeft = 1,
                        flexGrow = 1,
                    },
                };
                this.Add(three);
                {
                    // 名称标签
                    nameLabel = new Label()
                    {
                        name = "Name",
                        pickingMode = PickingMode.Ignore,
                        style =
                        {
                            display = DisplayStyle.Flex,
                            flexGrow = 1,
                            height = 18,
                            unityTextAlign = TextAnchor.MiddleLeft,
                        },
                    };
                    three.Add(nameLabel);
                    // 定位按钮
                    locateButton = new Button()
                    {
                        name = "Locate",
                        style =
                        {
                            display = DisplayStyle.None,
                            flexGrow = 0,
                            width = 16,
                            height = 16,
                            paddingTop = 0,
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            marginTop = StyleKeyword.Auto,
                            marginBottom = StyleKeyword.Auto,
                            marginLeft = StyleKeyword.Auto,
                            marginRight = StyleKeyword.Auto,
                            alignItems = Align.Center,
                            alignSelf = Align.Center,
                            justifyContent = Justify.Center,
                            position = Position.Absolute,
                            right = 2,
                        },
                    };
                    // 图标
                    locateButton.Add(new Image()
                    {
                        // image = ProjectPinBoardUtil.GetIcon("d_Import"),
                        image = ProjectPinBoardUtil.GetIcon("d_Record Off"),
                        // image = ProjectPinBoardUtil.GetIcon("d_Record On"),
                        // image = ProjectPinBoardUtil.GetIcon("d_RectTransform Icon"),
                        // image = ProjectPinBoardUtil.GetIcon("d_AvatarCompass"),
                        scaleMode = ScaleMode.ScaleToFit,
                        style =
                        {
                            width = 12,
                        },
                    });
                    locateButton.clicked += OnLocalButtonClick;
                    three.Add(locateButton);
                    // 名称输入框
                    nameTextField = new TextField()
                    {
                        name = "Input",
                        style =
                        {
                            display = DisplayStyle.None,
                            paddingLeft = 0,
                            marginTop = 1,
                            marginBottom = 1,
                            marginLeft = 0,
                            marginRight = 1,
                            minWidth = 100,
                        },
                    };
                    three.Add(nameTextField);
                }

                // 监听鼠标事件（实现拖拽）
                this.RegisterCallback<MouseDownEvent>(OnMouseDown);
                this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                this.RegisterCallback<MouseUpEvent>(OnMouseUp);

                // 监听鼠标事件（显示定位按钮）
                this.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
                this.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            }

            /// <summary>
            /// 设置
            /// </summary>
            /// <param name="itemInfo"></param>
            public void Set(ItemInfo itemInfo)
            {
                // 绑定数据
                this.userData = itemInfo;

                // 加载资源
                string assetPath = AssetDatabase.GUIDToAssetPath(itemInfo.guid);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                // 置顶
                this.topImage.style.display = itemInfo.top ? DisplayStyle.Flex : DisplayStyle.None;

                // 名称和图标
                if (asset == null)
                {
                    this.nameLabel.text = "<Missing Asset>";
                    this.iconImage.image = ProjectPinBoardUtil.GetAssetIcon(assetPath);
                }
                else
                {
                    string displayName = itemInfo.displayName;
                    this.nameLabel.text = (!string.IsNullOrWhiteSpace(displayName) ? $"{displayName}{k_DisplayNameSuffix}" : itemInfo.Name);
                    this.iconImage.image = AssetDatabase.GetCachedIcon(assetPath);
                }
            }

            #region Name TextField

            /// <summary>
            /// 名称输入框失焦回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnNameTextFieldFocusOut(FocusOutEvent evt)
            {
                // 隐藏输入框
                HideNameTextField();
            }

            /// <summary>
            /// 名称输入框按键回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnNameTextFieldKeyDown(KeyDownEvent evt)
            {
                // Enter
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    ApplyNameInput();
                    evt.StopPropagation();
                }
                // Esc || F2
                else if (evt.keyCode == KeyCode.Escape || evt.keyCode == KeyCode.F2)
                {
                    HideNameTextField();
                    evt.StopPropagation();
                }
            }

            /// <summary>
            /// 是否正在展示名称输入框
            /// </summary>
            public bool isShowingNameTextField => (nameTextField.style.display == DisplayStyle.Flex);

            /// <summary>
            /// 展示名称输入框
            /// </summary>
            public void ShowNameTextField()
            {
                if ((userData == null) || isShowingNameTextField) return;
                // 切换状态
                nameLabel.style.display = DisplayStyle.None;
                nameTextField.style.display = DisplayStyle.Flex;
                // 设置初始文本
                nameTextField.value = userData.Name;
                nameTextField.tooltip = userData.Name;
                // 聚焦并选中文本
                nameTextField.Focus();
                nameTextField.SelectAll();
                // 监听事件
                EditorApplication.delayCall += () =>
                {
                    nameTextField.RegisterCallback<KeyDownEvent>(OnNameTextFieldKeyDown);
                    nameTextField.RegisterCallback<FocusOutEvent>(OnNameTextFieldFocusOut);
                };
            }

            /// <summary>
            /// 隐藏名称输入框
            /// </summary>
            public void HideNameTextField()
            {
                if (!isShowingNameTextField) return;
                // 取消监听事件
                nameTextField.UnregisterCallback<KeyDownEvent>(OnNameTextFieldKeyDown);
                nameTextField.UnregisterCallback<FocusOutEvent>(OnNameTextFieldFocusOut);
                // 切换状态
                nameLabel.style.display = DisplayStyle.Flex;
                nameTextField.style.display = DisplayStyle.None;
                // 重置文本
                nameTextField.value = string.Empty;
                // 脱离并返还焦点
                nameTextField.Blur();
                focusController?.focusedElement.Focus();
            }

            /// <summary>
            /// 应用名称输入
            /// </summary>
            private void ApplyNameInput()
            {
                if (userData == null) return;
                string input = nameTextField.value;
                if (string.IsNullOrWhiteSpace(input) || input == userData.AssetName)
                {
                    ProjectPinBoardManager.RemoveDisplayName(userData.guid);
                    HideNameTextField();
                    return;
                }
                if (ProjectPinBoardManager.SetDisplayName(userData.guid, input))
                {
                    HideNameTextField();
                }
                else
                {
                    HideNameTextField();
                    focusController?.focusedElement.Blur();
                }
            }

            #endregion

            #region Dragging

            /// <summary>
            /// 是否在当前元素上按下鼠标
            /// </summary>
            private bool m_GotMouseDown = false;

            /// <summary>
            /// 鼠标按下回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseDown(MouseDownEvent evt)
            {
                if (evt.target == this && evt.button == 0)
                {
                    m_GotMouseDown = true;
                }
            }

            /// <summary>
            /// 鼠标松开回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseUp(MouseUpEvent evt)
            {
                if (m_GotMouseDown && evt.button == 0)
                {
                    m_GotMouseDown = false;
                }
            }

            /// <summary>
            /// 鼠标移动回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseMove(MouseMoveEvent evt)
            {
                if (m_GotMouseDown && evt.pressedButtons == 1)
                {
                    m_GotMouseDown = false;
                    if (HasOpenInstances())
                    {
                        ProjectPinBoardWindow window = GetOpenedInstance();
                        window.TriggerDragging();
                    }
                }
            }

            #endregion

            #region Locate Button

            /// <summary>
            /// 定位按钮点击回调
            /// </summary>
            private void OnLocalButtonClick()
            {
                if (userData == null) return;
                ProjectPinBoardUtil.SelectAsset(userData.guid);
            }

            /// <summary>
            /// 鼠标进入回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseEnter(MouseEnterEvent evt)
            {
                locateButton.style.display = isShowingNameTextField ? DisplayStyle.None : DisplayStyle.Flex;
            }

            /// <summary>
            /// 鼠标离开回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseLeave(MouseLeaveEvent evt)
            {
                locateButton.style.display = DisplayStyle.None;
            }

            #endregion

        }

        #endregion

        #region ListItem Menu

        /// <summary>
        /// 条目菜单名称
        /// </summary>
        private static class ListItemMenuItemName
        {
            public const string Select = "Select";
            public const string Open = "Open";
            public const string ShowInExplorer = "Show In Explorer";
            public const string Top = "Top";
            public const string UnTop = "Un-top";
            public const string RePin = "Re-pin (Update pin time)";
            public const string Unpin = "Unpin";
            public const string SetDisplayName = "Set Display Name";
            public const string SetDisplayNameToPath = "Set Display Name to Path";
            public const string RemoveDisplayName = "Remove Display Name";
            public const string SetTags = "Set Tag(s)";
            public const string RemoveTags = "Remove Tag(s)";
        }

        /// <summary>
        /// 创建条目菜单
        /// </summary>
        /// <param name="evt"></param>
        private void ItemMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            object listItem = evt.target;
            DropdownMenu menu = evt.menu;
            menu.AppendAction(ListItemMenuItemName.Select, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.Open, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.ShowInExplorer, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.RePin, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.Unpin, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.Top, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.UnTop, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.SetDisplayName, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.SetDisplayNameToPath, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.RemoveDisplayName, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.SetTags, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.RemoveTags, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();

            DropdownMenuAction.Status AlwaysEnabled(DropdownMenuAction a) => DropdownMenuAction.AlwaysEnabled(a);

            DropdownMenuAction.Status EnabledOnSingleSelection(DropdownMenuAction a)
            {
                return (m_ListView.selectedItems.Count() == 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }
        }

        /// <summary>
        /// 条目菜单行为回调
        /// </summary>
        /// <param name="action"></param>
        private void OnItemMenuAction(DropdownMenuAction action)
        {
            if (!(action.userData is ListItem item))
            {
                return;
            }
            switch (action.name)
            {
                case ListItemMenuItemName.Select:
                {
                    ItemInfo itemInfo = item.userData;
                    ProjectPinBoardUtil.FocusOnAsset(itemInfo.guid);
                    break;
                }
                case ListItemMenuItemName.Open:
                {
                    ItemInfo itemInfo = item.userData;
                    ProjectPinBoardUtil.OpenAsset(itemInfo.guid);
                    break;
                }
                case ListItemMenuItemName.ShowInExplorer:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        ProjectPinBoardUtil.ShowInExplorer(itemInfo.guid);
                    }
                    break;
                }
                case ListItemMenuItemName.RePin:
                {
                    ProjectPinBoardManager.Pin(GetSelectedItemGUIDs());
                    break;
                }
                case ListItemMenuItemName.Unpin:
                {
                    ProjectPinBoardManager.Unpin(GetSelectedItemGUIDs());
                    break;
                }
                case ListItemMenuItemName.Top:
                {
                    ProjectPinBoardManager.Top(GetSelectedItemGUIDs(), true);
                    break;
                }
                case ListItemMenuItemName.UnTop:
                {
                    ProjectPinBoardManager.Top(GetSelectedItemGUIDs(), false);
                    break;
                }
                case ListItemMenuItemName.SetDisplayName:
                {
                    item.ShowNameTextField();
                    break;
                }
                case ListItemMenuItemName.SetDisplayNameToPath:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        string path = itemInfo.Path.Replace("Assets/", "");
                        ProjectPinBoardManager.SetDisplayName(itemInfo.guid, path);
                    }
                    break;
                }
                case ListItemMenuItemName.RemoveDisplayName:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        ProjectPinBoardManager.RemoveDisplayName(itemInfo.guid);
                    }
                    break;
                }
                case ListItemMenuItemName.SetTags:
                {
                    ShowTagPopup(GetSelectedItemInfos());
                    break;
                }
                case ListItemMenuItemName.RemoveTags:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        ProjectPinBoardManager.RemoveTags(itemInfo.guid);
                    }
                    break;
                }
            }
        }

        #endregion

    }

}
