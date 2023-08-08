using System;
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
        /// 排序菜单
        /// </summary>
        private ToolbarMenu m_SortingMenu = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarSortingMenu()
        {
            m_SortingMenu = new ToolbarMenu()
            {
                name = "SortingMenu",
                tooltip = "Asset list sorting",
                variant = ToolbarMenu.Variant.Popup,
                focusable = false,
                style =
                {
                    flexShrink = 0,
                    width = 40,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 4,
                    paddingRight = 4,
                    marginLeft = -1,
                    marginRight = 0,
                    left = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.SpaceBetween,
                }
            };
            m_Toolbar.Add(m_SortingMenu);
            // 图标
            m_SortingMenu.Insert(0, new Image()
            {
                image = PipiUtility.GetIcon("AlphabeticalSorting"),
                scaleMode = ScaleMode.ScaleToFit,
            });
            // 隐藏文本元素
            {
                TextElement text = m_SortingMenu.Q<TextElement>("", "unity-text-element");
                text.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }

            // 构建下拉菜单项
            BuildSortingMenuItems();
        }

        #region Sorting Menu

        /// <summary>
        /// 当前排序类型
        /// </summary>
        private Sorting m_Sorting = Sorting.NameUp;

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
        /// 构建排序菜单下拉菜单项
        /// </summary>
        private void BuildSortingMenuItems()
        {
            DropdownMenu menu = m_SortingMenu.menu;
            foreach (var item in s_SortingMenuMap)
            {
                menu.AppendAction(item.Key, OnSortingMenuAction, GetSortingMenuActionStatus);
            }
        }

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
        /// 排序方式
        /// </summary>
        private enum Sorting
        {
            NameUp = 1,
            NameDown = 2,
            TimeUp = 3,
            TimeDown = 4,
        }

        /// <summary>
        /// 排序优先级
        /// </summary>
        private static class SortingPriority
        {
            public const int Directory = 20;
            public const int Top = 10;
            public const int Base = 0;
            public const int Invalid = -1;
        }

        /// <summary>
        /// 基础排序函数
        /// </summary>
        private static readonly Comparison<ItemInfo> s_BaseSortingComparer = (a, b) =>
        {
            int ap = SortingPriority.Base;
            int bp = SortingPriority.Base;
            // 是否置顶文件夹
            if (ProjectPinBoardSettings.topFolder)
            {
                if (a.IsDirectory()) ap += SortingPriority.Directory;
                if (b.IsDirectory()) bp += SortingPriority.Directory;
            }
            // 是否置顶
            if (a.top) ap += SortingPriority.Top;
            if (b.top) bp += SortingPriority.Top;
            // 是否为有效资源
            if (!a.IsValid()) ap += SortingPriority.Invalid;
            if (!b.IsValid()) bp += SortingPriority.Invalid;
            return bp - ap;
        };

        /// <summary>
        /// 排序函数
        /// </summary>
        private static readonly Dictionary<Sorting, Comparison<ItemInfo>> s_SortingComparers = new Dictionary<Sorting, Comparison<ItemInfo>>()
        {
            {
                Sorting.NameUp, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    return baseSorting != 0 ? baseSorting : string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
                }
            },
            {
                Sorting.NameDown, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return (-string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
                }
            },
            {
                Sorting.TimeUp, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return a.time.CompareTo(b.time);
                }
            },
            {
                Sorting.TimeDown, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return (-a.time.CompareTo(b.time));
                }
            },
        };

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="list"></param>
        private void Sort(ref List<ItemInfo> list)
        {
            list.Sort(s_SortingComparers[m_Sorting]);
        }

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
