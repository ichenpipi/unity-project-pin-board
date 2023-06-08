using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

namespace ChenPipi.ProjectPinBoard
{

    /// <summary>
    /// PinBoard 窗口（标签窗口）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 标签窗口背景遮罩
        /// </summary>
        private VisualElement m_TagPopupMask = null;

        /// <summary>
        /// 标签窗口
        /// </summary>
        private PopupWindow m_TagPopupWindow = null;

        /// <summary>
        /// 关闭标签窗口
        /// </summary>
        private void CloseTagPopup()
        {
            m_TagPopupWindow.userData = null;

            Box allTagsContainer = m_TagPopupWindow.Q<Box>("AllTags");
            foreach (VisualElement element in allTagsContainer.Children().ToArray())
            {
                if (element is Label) element.RemoveFromHierarchy();
            }

            Box itemTagsContainer = m_TagPopupWindow.Q<Box>("ItemTags");
            foreach (VisualElement element in itemTagsContainer.Children().ToArray())
            {
                if (element is Label) element.RemoveFromHierarchy();
            }
            ((List<string>)itemTagsContainer.userData)?.Clear();

            m_TagPopupWindow.style.display = DisplayStyle.None;
            m_TagPopupMask.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 展示标签窗口
        /// </summary>
        /// <param name="itemInfo"></param>
        private void ShowTagPopup(ItemInfo itemInfo)
        {
            // 创建底部遮罩
            if (m_TagPopupMask == null)
            {
                m_TagPopupMask = new VisualElement()
                {
                    name = "TagPopupMask",
                    style =
                    {
                        position = Position.Absolute,
                        top = 0,
                        bottom = 0,
                        left = 0,
                        right = 0,
                        backgroundColor = new Color(0f, 0f, 0f, 0.3f),
                    }
                };
                rootVisualElement.Add(m_TagPopupMask);
                // 点击遮罩关闭弹窗
                m_TagPopupMask.RegisterCallback<ClickEvent>((evt) => CloseTagPopup());
            }
            else
            {
                m_TagPopupMask.style.display = DisplayStyle.Flex;
                m_TagPopupMask.BringToFront();
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 创建窗口
            if (m_TagPopupWindow == null)
            {
                // 生成标题文本
                Label GenSubTitleLabel(string text)
                {
                    return new Label()
                    {
                        name = "SubTitle",
                        text = text,
                        style =
                        {
                            paddingLeft = 0,
                            paddingRight = 0,
                            marginTop = 0,
                            marginLeft = 5,
                            marginRight = 5,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            whiteSpace = WhiteSpace.Normal,
                        }
                    };
                }

                // 标签容器边框颜色
                Color tagContainerBorderColor = new Color(88 / 255f, 88 / 255f, 88 / 255f, 0.5f);
                // 标签容器边框宽度
                const int tagContainerBorderWidth = 1;
                // 标签容器边框圆角
                const int tagContainerBorderRadius = 5;

                // 生成标签容器
                Box GenTagContainer(string name)
                {
                    return new Box()
                    {
                        name = name,
                        style =
                        {
                            minHeight = 32,
                            paddingTop = 0,
                            paddingBottom = 5,
                            paddingLeft = 0,
                            paddingRight = 5,
                            marginTop = 2,
                            marginLeft = 3,
                            marginRight = 3,
                            borderTopWidth = tagContainerBorderWidth,
                            borderBottomWidth = tagContainerBorderWidth,
                            borderLeftWidth = tagContainerBorderWidth,
                            borderRightWidth = tagContainerBorderWidth,
                            borderTopColor = tagContainerBorderColor,
                            borderBottomColor = tagContainerBorderColor,
                            borderLeftColor = tagContainerBorderColor,
                            borderRightColor = tagContainerBorderColor,
                            borderTopLeftRadius = tagContainerBorderRadius,
                            borderTopRightRadius = tagContainerBorderRadius,
                            borderBottomLeftRadius = tagContainerBorderRadius,
                            borderBottomRightRadius = tagContainerBorderRadius,
                            flexDirection = FlexDirection.Row,
                            flexWrap = Wrap.Wrap,
                            flexGrow = 1,
                            flexShrink = 0,
                        },
                    };
                }

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                // 创建窗口元素
                m_TagPopupWindow = new PopupWindow()
                {
                    name = "TagPopup",
                    focusable = true,
                    style =
                    {
                        position = Position.Absolute,
                        paddingTop = 5,
                        paddingBottom = 10,
                        backgroundColor = new Color(57 / 255f, 57 / 255f, 57 / 255f, 1f),
                        borderTopLeftRadius = 8,
                        borderTopRightRadius = 8,
                        borderBottomLeftRadius = 8,
                        borderBottomRightRadius = 8,
                        flexDirection = FlexDirection.Column,
                    }
                };
                rootVisualElement.Add(m_TagPopupWindow);

                // 内部容器样式
                m_TagPopupWindow.contentContainer.style.paddingTop = 4;

                // 监听按键事件
                m_TagPopupWindow.RegisterCallback<KeyDownEvent>(evt =>
                {
                    // Enter
                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    {
                        OnConfirmButtonClick();
                    }
                    // Esc
                    else if (evt.keyCode == KeyCode.Escape)
                    {
                        OnCancelButtonClick();
                    }
                });

                // 更新窗口位置尺寸
                void UpdateTransform()
                {
                    Rect rootBound = rootVisualElement.worldBound;
                    const float width = 300;
                    const float top = 50f;
                    const float marginBottom = 20;
                    float left = (rootBound.width / 2f) - (width / 2f);
                    float maxHeight = (rootBound.height - top - marginBottom);
                    m_TagPopupWindow.style.width = width;
                    m_TagPopupWindow.style.maxHeight = maxHeight;
                    m_TagPopupWindow.style.top = top;
                    m_TagPopupWindow.style.left = left;
                }

                // 跟随主面板尺寸变化
                rootVisualElement.RegisterCallback<GeometryChangedEvent>((evt) => UpdateTransform());

                // 更新窗口位置尺寸
                UpdateTransform();

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                // 大标题
                m_TagPopupWindow.Add(new Label()
                {
                    name = "Title",
                    style =
                    {
                        marginBottom = 10,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        whiteSpace = WhiteSpace.Normal,
                    }
                });

                ScrollView scrollView = new ScrollView();
                m_TagPopupWindow.Add(scrollView);

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                // 已有标签
                {
                    // 标题
                    scrollView.Add(GenSubTitleLabel("Select Existing Tags (Click to add)"));
                    // 容器
                    Box allTagsContainer = GenTagContainer("AllTags");
                    scrollView.Add(allTagsContainer);
                    // 添加一个空的占位元素（在Unity2020中出现的Bug：没有子元素时容器元素的布局会出现异常，发生坍缩）
                    allTagsContainer.Add(new VisualElement() { name = "Placeholder" });
                }

                // 分隔线
                scrollView.Add(GenHorizontalSeparator(8));

                // 当前标签
                {
                    // 标题
                    scrollView.Add(GenSubTitleLabel("Current Tags (Click to remove)"));
                    // 容器
                    Box itemTagsContainer = GenTagContainer("ItemTags");
                    itemTagsContainer.userData = new List<string>();
                    scrollView.Add(itemTagsContainer);
                    // 添加一个空的占位元素（在Unity2020中出现的Bug：没有子元素时容器元素的布局会出现异常，发生坍缩）
                    itemTagsContainer.Add(new VisualElement() { name = "Placeholder" });
                }

                // 分割线
                scrollView.Add(GenHorizontalSeparator(8));

                // 添加标签
                {
                    // 标题
                    scrollView.Add(GenSubTitleLabel("Add New Tag (Use ',' to separate)"));
                    // 容器
                    VisualElement container = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row,
                            minHeight = 30,
                            maxHeight = 50,
                            marginTop = 2,
                        }
                    };
                    scrollView.Add(container);
                    // 标签输入框
                    TextField tagTextField = new TextField()
                    {
                        name = "TagTextField",
                        multiline = true,
                        style =
                        {
                            flexGrow = 1,
                            flexShrink = 1,
                            marginTop = 0,
                            marginBottom = 0,
                            unityTextAlign = TextAnchor.UpperLeft,
                            whiteSpace = WhiteSpace.Normal,
                        }
                    };
                    {
                        VisualElement textInput = tagTextField.Q<VisualElement>("unity-text-input");
                        textInput.style.unityTextAlign = TextAnchor.UpperLeft;
                    }
                    container.Add(tagTextField);
                    // 监听按键事件
                    tagTextField.RegisterCallback<KeyDownEvent>(evt =>
                    {
                        // Enter
                        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                        {
                            if (string.IsNullOrEmpty(tagTextField.value))
                            {
                                OnConfirmButtonClick();
                            }
                            else
                            {
                                OnAddTagButtonClick();
                            }
                            evt.StopPropagation();
                        }
                    });
                    // 添加按钮
                    Button addTagButton = new Button()
                    {
                        name = "AddTagButton",
                        text = "Add",
                        style =
                        {
                            width = 60,
                            marginTop = 0,
                            marginBottom = 0,
                            marginLeft = 0,
                        }
                    };
                    container.Add(addTagButton);
                    addTagButton.clicked += OnAddTagButtonClick;
                }

                // 分割线
                scrollView.Add(GenHorizontalSeparator(8));

                // 底部按钮
                {
                    // 容器
                    VisualElement mainButtons = new VisualElement()
                    {
                        name = "MainButtons",
                        style =
                        {
                            flexDirection = FlexDirection.Row,
                            minHeight = 30,
                            flexShrink = 0,
                        }
                    };
                    scrollView.Add(mainButtons);
                    // 取消按钮
                    Button cancelButton = new Button()
                    {
                        name = "CancelButton",
                        text = "Cancel",
                        style =
                        {
                            height = 30,
                            flexGrow = 1,
                            unityFontStyleAndWeight = FontStyle.Bold,
                        }
                    };
                    mainButtons.Add(cancelButton);
                    cancelButton.clicked += OnCancelButtonClick;
                    // 确认按钮
                    Button confirmButton = new Button()
                    {
                        name = "ConfirmButton",
                        text = "Confirm",
                        style =
                        {
                            height = 30,
                            flexGrow = 1,
                            unityFontStyleAndWeight = FontStyle.Bold,
                        }
                    };
                    mainButtons.Add(confirmButton);
                    confirmButton.clicked += OnConfirmButtonClick;
                }
            }
            else
            {
                m_TagPopupWindow.style.display = DisplayStyle.Flex;
                m_TagPopupWindow.BringToFront();
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 获取当前条目信息
            ItemInfo GetItemInfo()
            {
                return (ItemInfo)m_TagPopupWindow.userData;
            }

            // 获取当前标签
            List<string> GetItemTags()
            {
                return (List<string>)m_TagPopupWindow.Q<Box>("ItemTags").userData;
            }

            // 设置标题
            void SetTitle(string text)
            {
                Label titleLabel = m_TagPopupWindow.Q<Label>("Title");
                titleLabel.text = text;
            }

            // 刷新所有标签元素
            void RefreshAllTagsContainer(List<string> tags)
            {
                Box allTagsContainer = m_TagPopupWindow.Q<Box>("AllTags");

                foreach (VisualElement element in allTagsContainer.Children().ToArray())
                {
                    if (element is Label) element.RemoveFromHierarchy();
                }

                foreach (string tag in tags)
                {
                    Label label = GenTagLabel(tag, () =>
                    {
                        AddTag(tag);
                        FocusToTagTextField();
                    });
                    label.AddToClassList("Addable");
                    allTagsContainer.Add(label);
                }
            }

            // 刷新当前条目标签元素
            void RefreshItemTagsContainer(List<string> tags)
            {
                Box itemTagsContainer = m_TagPopupWindow.Q<Box>("ItemTags");

                List<string> itemTags = (List<string>)itemTagsContainer.userData;
                itemTags.Clear();
                tags.ForEach(v => itemTags.Add(v));

                foreach (VisualElement element in itemTagsContainer.Children().ToArray())
                {
                    if (element is Label) element.RemoveFromHierarchy();
                }

                foreach (string tag in itemTags)
                {
                    Label label = GenTagLabel(tag, () => RemoveTag(tag));
                    label.AddToClassList("Removable");
                    itemTagsContainer.Add(label);
                }
            }

            // 设置输入框内容
            void SetTagTextFieldValue(string text)
            {
                TextField tagTextField = m_TagPopupWindow.Q<TextField>("TagTextField");
                tagTextField.value = text;
            }

            // 聚焦到标签输入框
            void FocusToTagTextField()
            {
                TextField tagTextField = m_TagPopupWindow.Q<TextField>("TagTextField");
                if (m_TagPopupWindow.focusController.focusedElement == tagTextField)
                {
                    tagTextField.Blur();
                    EditorApplication.delayCall += () =>
                    {
                        tagTextField.Focus();
                        tagTextField.SelectAll();
                    };
                }
                else
                {
                    tagTextField.Focus();
                    tagTextField.SelectAll();
                }
            }

            // 添加标签按钮回调
            void OnAddTagButtonClick()
            {
                TextField tagTextField = m_TagPopupWindow.Q<TextField>("TagTextField");
                string[] texts = tagTextField.value.Split(new[] { ",", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                List<string> fails = new List<string>();
                foreach (string text in texts)
                {
                    if (!AddTag(text)) fails.Add(text);
                }
                tagTextField.value = fails.Join(", ");
                // 聚焦到标签输入框
                FocusToTagTextField();
            }

            // 确认按钮回调
            void OnConfirmButtonClick()
            {
                string guid = GetItemInfo().guid;
                List<string> itemTags = GetItemTags();
                ProjectPinBoardManager.SetTags(guid, itemTags);
                CloseTagPopup();
            }

            // 取消按钮回调
            void OnCancelButtonClick()
            {
                CloseTagPopup();
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 添加标签
            bool AddTag(string tag)
            {
                // 移除空格
                tag = Regex.Replace(tag, @"\s", "");

                // 是否空白
                if (string.IsNullOrWhiteSpace(tag)) return false;
                // 是否合法
                if (!ProjectPinBoardManager.IsValidTag(tag)) return false;

                // 是否已存在
                Box itemTagsContainer = m_TagPopupWindow.Q<Box>("ItemTags");
                List<string> itemTags = (List<string>)itemTagsContainer.userData;
                if (itemTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    ShowToast($"Tag \"{tag}\" already exists!");
                    return false;
                }

                // 添加数据
                itemTags.Add(tag);
                // 添加元素
                Label label = GenTagLabel(tag, () =>
                {
                    RemoveTag(tag);
                    FocusToTagTextField();
                });
                label.AddToClassList("Removable");
                itemTagsContainer.Add(label);

                return true;
            }

            // 移除标签
            void RemoveTag(string tag)
            {
                Box itemTagsContainer = m_TagPopupWindow.Q<Box>("ItemTags");

                // 更新数据
                List<string> itemTags = (List<string>)itemTagsContainer.userData;
                itemTags.Remove(tag);

                // 移除元素
                foreach (VisualElement element in itemTagsContainer.Children())
                {
                    if (!(element is Label label)) continue;
                    if (label.text == tag)
                    {
                        label.RemoveFromHierarchy();
                        break;
                    }
                }
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 保存条目信息引用
            m_TagPopupWindow.userData = itemInfo;

            // 更新标题
            SetTitle($"Set tag(s) of \"{itemInfo.AssetName}\"");

            // 刷新所有标签
            RefreshAllTagsContainer(m_ItemTagList);

            // 刷新当前条目标签
            RefreshItemTagsContainer(itemInfo.tags);

            // 清空标签输入
            SetTagTextFieldValue(string.Empty);

            // 聚焦到标签输入框
            FocusToTagTextField();
        }

    }

}
