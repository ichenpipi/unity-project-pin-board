using System;
using UnityEditor.UIElements;
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
        /// 初始化
        /// </summary>
        private void InitToolbarFilter()
        {
            // 类型
            {
                ToolbarButton button = new ToolbarButton()
                {
                    name = "TypeButton",
                    tooltip = "Filter by Type",
                    style =
                    {
                        width = 25,
                        minWidth = 25,
                        paddingTop = 0,
                        marginLeft = 0,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(button);
                // 图标
                button.Add(new Image()
                {
                    image = PipiUtility.GetIcon("d_FilterByType"),
                    scaleMode = ScaleMode.ScaleToFit,
                });
                // 回调
                button.clicked += () =>
                {
                    const string popupTitle = "Filter by Type";
                    Vector2 popupPos = new Vector2(button.worldBound.x, button.worldBound.y + 4);
                    ShowTogglePopup(popupTitle, popupPos, m_ItemTypeList, m_FilteringType, (v) =>
                    {
                        SetTypeFilter(m_FilteringType.Equals(v, StringComparison.OrdinalIgnoreCase) ? string.Empty : v);
                    });
                };
            }

            // 标签
            {
                ToolbarButton button = new ToolbarButton()
                {
                    name = "TagButton",
                    tooltip = "Filter by Tag",
                    style =
                    {
                        width = 25,
                        minWidth = 25,
                        paddingTop = 0,
                        marginLeft = -1,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(button);
                // 图标
                button.Add(new Image()
                {
                    image = PipiUtility.GetIcon("d_FilterByLabel"),
                    scaleMode = ScaleMode.ScaleToFit,
                });
                // 回调
                button.clicked += () =>
                {
                    const string popupTitle = "Filter by Tag";
                    Vector2 popupPos = new Vector2(button.worldBound.x, button.worldBound.y + 4);
                    ShowTogglePopup(popupTitle, popupPos, m_ItemTagList, m_FilteringTag, (v) =>
                    {
                        SetTagFilter(m_FilteringTag.Equals(v, StringComparison.OrdinalIgnoreCase) ? string.Empty : v);
                    });
                };
            }
        }

        #region Filtering

        /// <summary>
        /// 类型过滤正则表达式
        /// </summary>
        private const string k_TypeFilterPattern = @"(.*)\s*type:(\S+)?\s*(.*)";

        /// <summary>
        /// 标签过滤正则表达式
        /// </summary>
        private const string k_TagFilterPattern = @"(.*)\s*tag:(\S+)?\s*(.*)";

        /// <summary>
        /// 过滤类型
        /// </summary>
        private string m_FilteringType = string.Empty;

        /// <summary>
        /// 过滤标签
        /// </summary>
        private string m_FilteringTag = string.Empty;

        /// <summary>
        /// 设置搜索栏标签过滤
        /// </summary>
        /// <param name="type"></param>
        /// <param name="updateSearch"></param>
        private void SetTypeFilter(string type, bool updateSearch = true)
        {
            m_FilteringType = type;
            if (updateSearch)
            {
                string content = GetSearchContent();
                SetSearchText(string.IsNullOrWhiteSpace(type) ? content : $"type:{type} {content}");
            }
        }

        /// <summary>
        /// 设置搜索栏标签过滤
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="updateSearch"></param>
        private void SetTagFilter(string tag, bool updateSearch = true)
        {
            m_FilteringTag = tag;
            if (updateSearch)
            {
                string content = GetSearchContent();
                SetSearchText(string.IsNullOrWhiteSpace(tag) ? content : $"tag:{tag} {content}");
            }
        }

        #endregion

    }

}
