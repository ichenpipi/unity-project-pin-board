using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（工具栏）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 工具栏
        /// </summary>
        private VisualElement m_Toolbar = null;

        /// <summary>
        /// 工具栏搜索栏
        /// </summary>
        private ToolbarSearchField m_ToolbarSearchField = null;

        /// <summary>
        /// 工具栏文件夹置顶开关
        /// </summary>
        private ToolbarToggle m_ToolbarTopFolderToggle = null;

        /// <summary>
        /// 工具栏预览开关
        /// </summary>
        private ToolbarToggle m_ToolbarPreviewToggle = null;

        /// <summary>
        /// 工具栏同步选择开关
        /// </summary>
        private ToolbarToggle m_ToolbarSyncSelectionToggle = null;

        /// <summary>
        /// 初始化工具栏
        /// </summary>
        private void InitToolbar()
        {
            // 绑定视图
            m_Toolbar = rootVisualElement.Q<VisualElement>("Toolbar");
            {
                m_Toolbar.style.height = 20;
                m_Toolbar.style.flexShrink = 0;
                m_Toolbar.style.flexDirection = FlexDirection.Row;
            }

            // 搜索栏
            {
                m_ToolbarSearchField = new ToolbarSearchField()
                {
                    name = "SearchField",
                    value = m_SearchText,
                    style =
                    {
                        width = StyleKeyword.Auto,
                        maxWidth = 3000,
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
                        FocusToContentListView();
                    }
                });
            }

            // 元素宽度
            const int elementWidth = 25;

            // 类型按钮
            {
                ToolbarButton button = new ToolbarButton()
                {
                    name = "TypeButton",
                    tooltip = "Filter by Type",
                    style =
                    {
                        width = elementWidth,
                        minWidth = elementWidth,
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
                    image = ProjectPinBoardUtil.GetIcon("d_FilterByType"),
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

            // 标签按钮
            {
                ToolbarButton button = new ToolbarButton()
                {
                    name = "TagButton",
                    tooltip = "Filter by Tag",
                    style =
                    {
                        width = elementWidth,
                        minWidth = elementWidth,
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
                    image = ProjectPinBoardUtil.GetIcon("d_FilterByLabel"),
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

            // 文件夹置顶开关
            {
                m_ToolbarTopFolderToggle = new ToolbarToggle()
                {
                    name = "FolderToggle",
                    tooltip = "Keep folders on top",
                    value = ProjectPinBoardSettings.topFolder,
                    style =
                    {
                        width = elementWidth,
                        minWidth = elementWidth,
                        paddingTop = 0,
                        marginLeft = -1,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(m_ToolbarTopFolderToggle);
                // 图标
                m_ToolbarTopFolderToggle.Add(new Image()
                {
                    image = ProjectPinBoardUtil.GetIcon("d_Folder Icon"),
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        flexBasis = Length.Percent(100),
                        width = 12,
                    }
                });
                // 回调
                m_ToolbarTopFolderToggle.RegisterValueChangedCallback(OnFolderToggleValueChanged);
            }

            // 预览开关
            {
                m_ToolbarPreviewToggle = new ToolbarToggle()
                {
                    name = "PreviewToggle",
                    tooltip = "Enable preview panel",
                    value = ProjectPinBoardSettings.enablePreview,
                    style =
                    {
                        width = elementWidth,
                        minWidth = elementWidth,
                        paddingTop = 0,
                        marginLeft = -1,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(m_ToolbarPreviewToggle);
                // 图标
                m_ToolbarPreviewToggle.Add(new Image()
                {
                    image = ProjectPinBoardUtil.GetIcon("d_scenevis_visible_hover"),
                    scaleMode = ScaleMode.ScaleToFit,
                });
                // 回调
                m_ToolbarPreviewToggle.RegisterValueChangedCallback(OnPreviewToggleValueChanged);
            }

            // 同步选择开关
            {
                m_ToolbarSyncSelectionToggle = new ToolbarToggle()
                {
                    name = "SyncSelection",
                    tooltip = "Sync selection to Project Browser",
                    value = ProjectPinBoardSettings.syncSelection,
                    style =
                    {
                        width = elementWidth,
                        minWidth = elementWidth,
                        paddingTop = 0,
                        marginLeft = -1,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(m_ToolbarSyncSelectionToggle);
                // 图标
                m_ToolbarSyncSelectionToggle.Add(new Image()
                {
                    image = ProjectPinBoardUtil.GetIcon("d_Grid.Default"),
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        alignSelf = Align.Center,
                        width = 16,
                    }
                });
                // 回调
                m_ToolbarSyncSelectionToggle.RegisterValueChangedCallback(OnSyncSelectionValueChanged);
            }

            // 排序菜单
            {
                ToolbarMenu sortingMenu = new ToolbarMenu()
                {
                    name = "SortingMenu",
                    tooltip = "Sorting",
                    variant = ToolbarMenu.Variant.Popup,
                    style =
                    {
                        width = 38,
                        minWidth = 38,
                        paddingTop = 0,
                        paddingLeft = 0,
                        marginLeft = -2,
                        marginRight = -2,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                    }
                };
                m_Toolbar.Add(sortingMenu);
                // 图标
                sortingMenu.Insert(0, new Image()
                {
                    image = ProjectPinBoardUtil.GetIcon("d_AlphabeticalSorting"),
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

        /// <summary>
        /// 搜索栏内容变化回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            SetSearch(evt.newValue);
            UpdateContent();
        }

        /// <summary>
        /// 文件夹置顶开关回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnFolderToggleValueChanged(ChangeEvent<bool> evt)
        {
            ProjectPinBoardSettings.topFolder = evt.newValue;
            UpdateContent();
        }

        /// <summary>
        /// 预览区域开关回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnPreviewToggleValueChanged(ChangeEvent<bool> evt)
        {
            ProjectPinBoardSettings.enablePreview = evt.newValue;
            TogglePreview(evt.newValue);
        }

        /// <summary>
        /// 同步选择开关回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnSyncSelectionValueChanged(ChangeEvent<bool> evt)
        {
            ProjectPinBoardSettings.syncSelection = evt.newValue;
        }

        /// <summary>
        /// 聚焦到搜索框
        /// </summary>
        private void FocusToToolbarSearchField()
        {
            m_ToolbarSearchField.Focus();
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

    }

}
