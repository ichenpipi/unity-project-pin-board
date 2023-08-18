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
        /// 搜索栏
        /// </summary>
        private ToolbarSearchField m_ToolbarSearchField = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarSearchField()
        {
            m_ToolbarSearchField = new ToolbarSearchField()
            {
                name = "Search",
                value = m_SearchText,
                tooltip = "Search [Ctrl+F]",
                style =
                {
                    width = StyleKeyword.Auto,
                    marginLeft = 4,
                    marginRight = 4,
                    flexShrink = 1,
                }
            };
            m_Toolbar.Add(m_ToolbarSearchField);
            // 值变化回调
            m_ToolbarSearchField.RegisterValueChangedCallback(OnSearchFieldValueChanged);
            // 监听键盘事件
            m_ToolbarSearchField.RegisterCallback<KeyDownEvent>((evt) =>
            {
                // ↑ || ↓
                if (evt.keyCode == KeyCode.UpArrow || evt.keyCode == KeyCode.DownArrow)
                {
                    FocusToAssetList();
                }
            });
        }

        /// <summary>
        /// 搜索栏内容变化回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            SetSearchText(evt.newValue);
        }

        #region SearchField

        /// <summary>
        /// 搜索文本
        /// </summary>
        private string m_SearchText = string.Empty;

        /// <summary>
        /// 设置搜索栏内容
        /// </summary>
        /// <param name="value"></param>
        private void SetSearchText(string value)
        {
            m_SearchText = value;
            m_ToolbarSearchField.SetValueWithoutNotify(value);
            UpdateContent();
        }

        /// <summary>
        /// 聚焦到搜索框
        /// </summary>
        private void FocusToSearchField()
        {
            m_ToolbarSearchField.Focus();
        }

        #endregion

        #region Searching

        /// <summary>
        /// 获取不包含过滤器的实际搜索内容
        /// </summary>
        private string GetSearchContent()
        {
            string text = m_SearchText;
            if (text.Contains("type:"))
            {
                Match match = Regex.Match(text, k_TypeFilterPattern, RegexOptions.IgnoreCase);
                text = match.Groups[3].Value;
            }
            if (text.Contains("tag:"))
            {
                Match match = Regex.Match(text, k_TagFilterPattern, RegexOptions.IgnoreCase);
                text = match.Groups[3].Value;
            }
            return text;
        }

        #endregion

    }

}
