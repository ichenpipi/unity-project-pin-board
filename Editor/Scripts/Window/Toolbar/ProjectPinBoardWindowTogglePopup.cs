using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口：开关列表弹窗
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 展示开关列表弹窗
        /// </summary>
        /// <param name="headline"></param>
        /// <param name="pos"></param>
        /// <param name="labels"></param>
        /// <param name="curLabel"></param>
        /// <param name="toggleCallback"></param>
        private void ShowTogglePopup(string headline, Vector2 pos, List<string> labels, string curLabel, Action<string> toggleCallback)
        {

            // 创建底部遮罩
            VisualElement popupMask = new VisualElement()
            {
                name = "TogglePopupMask",
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    bottom = 0,
                    left = 0,
                    right = 0,
                }
            };
            rootVisualElement.Add(popupMask);

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 创建弹窗
            const int borderRadius = 8;
            PopupWindow popupWindow = new PopupWindow()
            {
                name = "TogglePopup",
                text = headline,
                style =
                {
                    position = Position.Absolute,
                    borderTopLeftRadius = borderRadius,
                    borderTopRightRadius = borderRadius,
                    borderBottomLeftRadius = borderRadius,
                    borderBottomRightRadius = borderRadius,
                }
            };
            rootVisualElement.Add(popupWindow);

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 更新窗口位置尺寸
            void UpdateTransform()
            {
                Rect rootBound = rootVisualElement.worldBound;
                const float minWidth = 150;
                const float maxWidth = 200;
                const float marginRight = 15;
                const float marginBottom = 15;
                float top = pos.y;
                float left = pos.x;
                float width = popupWindow.worldBound.width;
                if (left + width >= rootBound.width - marginRight)
                {
                    left = (rootBound.width - width - marginRight);
                }
                float maxHeight = (rootBound.height - top - marginBottom);
                popupWindow.style.minWidth = minWidth;
                popupWindow.style.maxWidth = maxWidth;
                popupWindow.style.maxHeight = maxHeight;
                popupWindow.style.top = top;
                popupWindow.style.left = left;
            }

            // 监听主面板尺寸变化
            rootVisualElement.RegisterCallback<GeometryChangedEvent>((evt) => UpdateTransform());
            // 监听自身元素变化
            popupWindow.RegisterCallback<GeometryChangedEvent>((evt) => UpdateTransform());

            // 更新窗口位置尺寸
            UpdateTransform();

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 关闭弹窗函数
            void CloseTogglePopup()
            {
                popupWindow.RemoveFromHierarchy();
                popupMask.RemoveFromHierarchy();
            }

            // 点击遮罩关闭弹窗
            popupMask.RegisterCallback<ClickEvent>((evt) => CloseTogglePopup());

            // 弹窗就绪后聚焦
            popupWindow.RegisterCallback<GeometryChangedEvent>((evt) => popupWindow.BringToFront());

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 开关变化回调
            void OnToggleValueChanged(ChangeEvent<bool> evt)
            {
                Toggle toggle = (Toggle)evt.target;
                string value = (string)toggle.userData;
                toggleCallback(value);
                CloseTogglePopup();
            }

            // 生成开关
            Toggle GenToggle(string label, bool isOn)
            {
                Toggle toggle = new Toggle()
                {
                    label = label,
                    value = isOn,
                    userData = label,
                    style =
                    {
                        minHeight = 16,
                        marginLeft = 0,
                        marginRight = 0,
                        paddingLeft = 1,
                        paddingRight = 1,
                        flexGrow = 1,
                    }
                };
                toggle.AddToClassList("Toggle");
                // 调整文本样式
                {
                    Label labelElement = toggle.labelElement;
                    labelElement.style.flexGrow = 1;
                    labelElement.style.flexShrink = 1;
                    labelElement.style.minWidth = StyleKeyword.Auto;
                    labelElement.style.whiteSpace = WhiteSpace.Normal;
                }
                // 调整开关样式
                {
                    VisualElement checkmarkElement = toggle.Q<VisualElement>("unity-checkmark").parent;
                    checkmarkElement.style.flexGrow = 0;
                    checkmarkElement.style.marginLeft = 5;
                }
                // 注册开关变化回调
                toggle.RegisterValueChangedCallback(OnToggleValueChanged);
                return toggle;
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // 内容
            if (labels.Count > 0)
            {
                // 创建滚动视图
                ScrollView scrollView = new ScrollView();
                popupWindow.Add(scrollView);
                // 添加开关
                foreach (string label in labels)
                {
                    bool isOn = label.Equals(curLabel, StringComparison.OrdinalIgnoreCase);
                    Toggle toggle = GenToggle(label, isOn);
                    scrollView.Add(toggle);
                }
            }
            else
            {
                // 添加占位符
                popupWindow.Add(new Label()
                {
                    name = "Placeholder",
                    text = "Empty...",
                    style =
                    {
                        color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f),
                        unityTextAlign = TextAnchor.MiddleCenter,
                    }
                });
            }
        }

    }

}
