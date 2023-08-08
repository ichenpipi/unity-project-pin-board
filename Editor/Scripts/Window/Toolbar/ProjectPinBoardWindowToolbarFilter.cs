using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        /// 类型过滤按钮
        /// </summary>
        private ToolbarButton m_TypeFilterButton = null;

        /// <summary>
        /// 标签过滤按钮
        /// </summary>
        private ToolbarButton m_TagFilterButton = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarFilterButton()
        {
            // 类型过滤
            m_TypeFilterButton = new ToolbarButton()
            {
                name = "TypeFilterButton",
                tooltip = "Filter by Type",
                focusable = false,
                style =
                {
                    flexShrink = 0,
                    width = 25,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 2,
                    paddingRight = 2,
                    marginLeft = 0,
                    marginRight = 0,
                    left = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_Toolbar.Add(m_TypeFilterButton);
            // 图标
            m_TypeFilterButton.Add(new Image()
            {
                image = PipiUtility.GetIcon("FilterByType"),
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                }
            });
            // 回调
            m_TypeFilterButton.clicked += OnTypeFilterButtonClicked;

            // 标签过滤
            m_TagFilterButton = new ToolbarButton()
            {
                name = "TagFilterButton",
                tooltip = "Filter by Tag",
                focusable = false,
                style =
                {
                    flexShrink = 0,
                    width = 25,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 2,
                    paddingRight = 2,
                    marginLeft = -1,
                    marginRight = 0,
                    left = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_Toolbar.Add(m_TagFilterButton);
            // 图标
            m_TagFilterButton.Add(new Image()
            {
                image = PipiUtility.GetIcon("FilterByLabel"),
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                }
            });
            // 回调
            m_TagFilterButton.clicked += OnTagFilterButtonClicked;
        }

        private void OnTypeFilterButtonClicked()
        {
            const string popupTitle = "Filter by Type";
            Vector2 popupPos = new Vector2(m_TypeFilterButton.worldBound.x, m_TypeFilterButton.worldBound.y + 4);
            ShowTogglePopup(popupTitle, popupPos, m_ItemTypeList, m_FilteringType, (v) => { SetTypeFilter(m_FilteringType.Equals(v, StringComparison.OrdinalIgnoreCase) ? string.Empty : v); });
        }

        private void OnTagFilterButtonClicked()
        {
            const string popupTitle = "Filter by Tag";
            Vector2 popupPos = new Vector2(m_TagFilterButton.worldBound.x, m_TagFilterButton.worldBound.y + 4);
            ShowTogglePopup(popupTitle, popupPos, m_ItemTagList, m_FilteringTag, (v) => { SetTagFilter(m_FilteringTag.Equals(v, StringComparison.OrdinalIgnoreCase) ? string.Empty : v); });
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

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void Filter(ref List<ItemInfo> list)
        {
            // 移除空格
            string text = m_SearchText.Trim();

            // 匹配类型
            SetTypeFilter(string.Empty, false);
            if (text.Contains("type:"))
            {
                Match match = Regex.Match(text, k_TypeFilterPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    SetTypeFilter(match.Groups[2].Value, false);
                    if (!string.IsNullOrWhiteSpace(m_FilteringType))
                    {
                        list = list.FindAll(v => v.MatchType(m_FilteringType));
                    }
                    text = match.Groups[3].Value;
                }
            }

            // 匹配标签
            SetTagFilter(string.Empty, false);
            if (text.Contains("tag:"))
            {
                Match match = Regex.Match(text, k_TagFilterPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    SetTagFilter(match.Groups[2].Value, false);
                    if (!string.IsNullOrWhiteSpace(m_FilteringTag))
                    {
                        list = list.FindAll(v => v.MatchTag(m_FilteringTag));
                    }
                    text = match.Groups[3].Value;
                }
            }

            // 匹配名称
            if (!string.IsNullOrWhiteSpace(text))
            {
                string pattern = text.Trim().ToCharArray().Join(".*");
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                list = list.FindAll(v => regex.Match(v.AssetName).Success || regex.Match(v.displayName).Success);
            }
        }

        #endregion

    }

}
