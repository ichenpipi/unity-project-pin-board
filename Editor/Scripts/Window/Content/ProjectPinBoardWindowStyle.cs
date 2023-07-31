using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region TagLabel

        /// <summary>
        /// 标签边框圆角半径
        /// </summary>
        private const int k_TagBorderRadius = 9;

        /// <summary>
        /// 生成标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="clickCallback"></param>
        /// <returns></returns>
        private Label GenTagLabel(string text, Action clickCallback = null)
        {
            Label label = new Label()
            {
                name = $"Tag:{text}",
                text = text,
                style =
                {
                    alignSelf = Align.Center,
                    flexShrink = 1,
                    minWidth = 20,
                    minHeight = 20,
                    borderTopLeftRadius = k_TagBorderRadius,
                    borderTopRightRadius = k_TagBorderRadius,
                    borderBottomLeftRadius = k_TagBorderRadius,
                    borderBottomRightRadius = k_TagBorderRadius,
                    paddingLeft = 5,
                    paddingRight = 5,
                    marginLeft = 5,
                    marginTop = 5,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    whiteSpace = WhiteSpace.Normal,
                }
            };
            // 使用 uss 指定部分样式，代码目前无法指定 hover 状态样式
            label.AddToClassList("Tag");
            // 点击回调
            if (clickCallback != null)
            {
                label.RegisterCallback<MouseDownEvent>(_ => clickCallback());
            }
            return label;
        }

        #endregion

        #region Separator

        /// <summary>
        /// 分割线颜色
        /// </summary>
        private readonly Color m_SeparatorColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1f);

        /// <summary>
        /// 生成水平分割线
        /// </summary>
        /// <param name="margin"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private VisualElement GenHorizontalSeparator(float margin = 5, string name = "Separator")
        {
            return new VisualElement()
            {
                name = name,
                style =
                {
                    height = 1,
                    borderBottomWidth = 1,
                    borderBottomColor = m_SeparatorColor,
                    marginTop = margin,
                    marginBottom = margin,
                    flexShrink = 0,
                },
            };
        }

        #endregion

    }

}
