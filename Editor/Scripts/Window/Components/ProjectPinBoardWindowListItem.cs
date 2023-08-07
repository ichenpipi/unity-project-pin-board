using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

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
            /// 名称输入框
            /// </summary>
            public readonly TextField nameTextField = null;

            /// <summary>
            /// 浮动按钮
            /// </summary>
            public readonly ButtonWithIcon floatButton = null;

            /// <summary>
            /// 启用浮动按钮
            /// </summary>
            public bool enableFloatButton = false;

            /// <summary>
            /// 浮动按钮点击回调
            /// </summary>
            public event Action<ListItem> floatButtonClicked;

            /// <summary>
            /// 重命名回调
            /// </summary>
            public Func<ListItem, string, bool> renameCallback;

            /// <summary>
            /// 拖拽回调
            /// </summary>
            public event Action<ListItem> dragged;

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
                        image = PipiUtility.GetIcon("Download-Available"),
                        pickingMode = PickingMode.Ignore,
                        scaleMode = ScaleMode.ScaleToFit,
                        style =
                        {
                            display = DisplayStyle.None,
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
                        alignItems = Align.Center,
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
                    // 浮动按钮
                    floatButton = new ButtonWithIcon()
                    {
                        name = "FloatButton",
                        focusable = false,
                        style =
                        {
                            position = Position.Absolute,
                            right = 2,
                            width = 16,
                            height = 16,
                            display = DisplayStyle.None,
                        },
                    };
                    // 点击回调
                    floatButton.clicked += OnFloatButtonClick;
                    three.Add(floatButton);
                    // 名称输入框
                    nameTextField = new TextField()
                    {
                        name = "NameTextField",
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

                // 监听鼠标事件（显示浮动按钮）
                this.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
                this.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

                // 监听鼠标事件（实现拖拽）
                this.RegisterCallback<MouseDownEvent>(OnMouseDown);
                this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                this.RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            #region Interface

            public void SetText(string text)
            {
                this.nameLabel.text = text;
            }

            public void SetTextFontStyle(StyleEnum<FontStyle> fontStyle)
            {
                this.nameLabel.style.unityFontStyleAndWeight = fontStyle;
            }

            public void SetIcon(Texture image)
            {
                this.iconImage.image = image;
            }

            public void SetTop(bool top)
            {
                this.topImage.style.display = top ? DisplayStyle.Flex : DisplayStyle.None;
            }

            #endregion

            #region Name TextField

            /// <summary>
            /// 名称输入框失焦回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnNameTextFieldFocusOut(FocusOutEvent evt)
            {
                ApplyNameInput();
            }

            /// <summary>
            /// 名称输入框按键回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnNameTextFieldKeyDown(KeyDownEvent evt)
            {
                bool stopEvent = true;

                // Enter
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    ApplyNameInput();
                }
                // Esc || F2
                else if (evt.keyCode == KeyCode.Escape || evt.keyCode == KeyCode.F2)
                {
                    HideNameTextField();
                }
                // 不响应
                else
                {
                    stopEvent = false;
                }

                if (stopEvent)
                {
                    // 阻止事件的默认行为，停止事件传播
                    evt.PreventDefault();
                    evt.StopImmediatePropagation();
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
                if (isShowingNameTextField) return;
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
            }

            /// <summary>
            /// 应用名称输入
            /// </summary>
            private void ApplyNameInput()
            {
                if (renameCallback == null || renameCallback.Invoke(this, nameTextField.value))
                {
                    HideNameTextField();
                }
            }

            #endregion

            #region Float Button

            /// <summary>
            /// 浮动按钮点击回调
            /// </summary>
            private void OnFloatButtonClick()
            {
                floatButtonClicked?.Invoke(this);
            }

            /// <summary>
            /// 鼠标进入回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseEnter(MouseEnterEvent evt)
            {
                if (!enableFloatButton || isShowingNameTextField)
                {
                    floatButton.style.display = DisplayStyle.None;
                }
                else
                {
                    floatButton.style.display = DisplayStyle.Flex;
                }
            }

            /// <summary>
            /// 鼠标离开回调
            /// </summary>
            /// <param name="evt"></param>
            private void OnMouseLeave(MouseLeaveEvent evt)
            {
                floatButton.style.display = DisplayStyle.None;
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
                    dragged?.Invoke(this);
                }
            }

            #endregion

        }

    }

}
