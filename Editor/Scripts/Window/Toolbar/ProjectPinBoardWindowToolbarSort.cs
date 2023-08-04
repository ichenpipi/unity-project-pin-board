using System.Collections.Generic;
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
        private void InitToolbarSort()
        {
            // 排序菜单
            {
                ToolbarMenu sortingMenu = new ToolbarMenu()
                {
                    name = "SortingMenu",
                    tooltip = "Sorting",
                    variant = ToolbarMenu.Variant.Popup,
                    style =
                    {
                        flexShrink = 0,
                        width = 40,
                        paddingLeft = 3,
                        paddingRight = 4,
                        marginLeft = -2,
                        marginRight = -1,
                        alignItems = Align.Center,
                        justifyContent = Justify.SpaceBetween,
                    }
                };
                m_Toolbar.Add(sortingMenu);
                // 图标
                sortingMenu.Insert(0, new Image()
                {
                    image = PipiUtility.GetIcon("d_AlphabeticalSorting"),
                    scaleMode = ScaleMode.ScaleToFit,
                });
                // 文本
                TextElement text = sortingMenu.Q<TextElement>("", "unity-text-element");
                text.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                // 下拉菜单
                DropdownMenu menu = sortingMenu.menu;
                foreach (var item in s_SortingMenuMap)
                {
                    menu.AppendAction(item.Key, OnSortingMenuAction, GetSortingMenuActionStatus);
                }
            }
        }

        #region Sorting Menu

        /// <summary>
        /// 排序菜单表
        /// </summary>
        private static readonly Dictionary<string, Sorting> s_SortingMenuMap = new Dictionary<string, Sorting>()
        {
            { "Name ↑", Sorting.NameUp },
            { "Name ↓", Sorting.NameDown },
            { "Pin Time ↑", Sorting.TimeUp },
            { "Pin Time ↓", Sorting.TimeDown },
        };

        /// <summary>
        /// 排序菜单行为回调
        /// </summary>
        /// <param name="action"></param>
        private void OnSortingMenuAction(DropdownMenuAction action)
        {
            if (s_SortingMenuMap.TryGetValue(action.name, out Sorting value))
            {
                SwitchSorting(value);
            }
        }

        /// <summary>
        /// 获取排序菜单行为状态
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private DropdownMenuAction.Status GetSortingMenuActionStatus(DropdownMenuAction action)
        {
            if (s_SortingMenuMap.TryGetValue(action.name, out Sorting value))
            {
                return (m_Sorting == value ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }
            return DropdownMenuAction.Status.Disabled;
        }

        #endregion

        #region Sorting

        /// <summary>
        /// 当前排序类型
        /// </summary>
        private Sorting m_Sorting = Sorting.NameUp;

        /// <summary>
        /// 切换排序
        /// </summary>
        /// <param name="sorting"></param>
        private void SwitchSorting(Sorting sorting)
        {
            m_Sorting = sorting;
            UpdateContent();
        }

        #endregion

    }

}
