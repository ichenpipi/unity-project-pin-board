using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region Initialization

        /// <summary>
        /// 列表
        /// </summary>
        private ListView m_AssetList = null;

        /// <summary>
        /// 当前选中的第一个条目
        /// </summary>
        private string m_FirstSelectedItemGuid = null;

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitListView()
        {
            m_AssetList = new ListView()
            {
                name = "ListView",
#if UNITY_2021_1_OR_NEWER
                fixedItemHeight = 18,
#else
                itemHeight = 18,
#endif
                selectionType = SelectionType.Multiple,
                makeItem = CreateListItem,
                bindItem = BindListItem,
                unbindItem = UnbindListItem,
                style =
                {
                    flexBasis = Length.Percent(100),
                    minWidth = 100,
                }
            };
            m_ContentSplitView.Add(m_AssetList);

            // 列表选择变化回调
            m_AssetList.onSelectionChange += OnAssetListSelectionChange;
            // 列表条目选择（双击）回调
            m_AssetList.onItemsChosen += OnAssetListItemsChosen;
        }

        /// <summary>
        /// 列表选择变化回调
        /// </summary>
        /// <param name="objs"></param>
        private void OnAssetListSelectionChange(IEnumerable<object> objs)
        {
            object[] infos = objs as object[] ?? objs.ToArray();
            if (!infos.Any())
            {
                // 清除预览
                ClearPreview();
                return;
            }
            // 选中条目列表中包含当前条目，不做切换
            int index = infos.ToList().FindIndex(o => ((ItemInfo)o).MatchGuid(m_FirstSelectedItemGuid));
            if (index >= 0)
            {
                return;
            }
            // 缓存首个选择，用于热重载后恢复选中
            ItemInfo itemInfo = (ItemInfo)m_AssetList.selectedItems.First();
            m_FirstSelectedItemGuid = itemInfo.guid;
            // 设置预览
            SetPreview(itemInfo);
            // 同步选择
            if (ProjectPinBoardSettings.syncSelection)
            {
                PipiUtility.SelectAsset(itemInfo.guid);
                // 夺回焦点
                this.Focus();
            }
        }

        /// <summary>
        /// 列表条目选择（双击）回调
        /// </summary>
        /// <param name="objs"></param>
        private void OnAssetListItemsChosen(IEnumerable<object> objs)
        {
            object[] infos = objs as object[] ?? objs.ToArray();
            if (!infos.Any())
            {
                return;
            }
            ItemInfo info = (ItemInfo)infos.First();
            string guid = info.guid;
            // 是否可以打开
            if (PipiUtility.CanOpenInEditor(guid) || PipiUtility.CanOpenInScriptEditor(guid))
            {
                // 打开资源
                PipiUtility.OpenAsset(guid);
            }
            else
            {
                // 选中资源
                PipiUtility.FocusOnAsset(guid);
            }
        }

        #endregion

        #region Interface

        /// <summary>
        /// 当前列表数据
        /// </summary>
        private List<ItemInfo> m_AssetListData = new List<ItemInfo>();

        /// <summary>
        /// 更新列表
        /// </summary>
        private void UpdateAssetList()
        {
            // 克隆数据副本
            m_AssetListData.Clear();
            m_AssetListData.AddRange(ProjectPinBoardData.items);

            // 处理数据
            if (m_AssetListData.Count > 0)
            {
                // 过滤
                Filter(ref m_AssetListData);
                // 排序
                Sort(ref m_AssetListData);
            }

            // 列表数据
            m_AssetList.itemsSource = m_AssetListData;
#if UNITY_2021_2_OR_NEWER
            m_AssetList.Rebuild();
#else
            m_AssetList.Refresh();
#endif
        }

        /// <summary>
        /// 聚焦到列表
        /// </summary>
        public void FocusToAssetList()
        {
            m_AssetList.Focus();
        }

        /// <summary>
        /// 选中条目
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="notify"></param>
        public void SetAssetListSelection(string guid, bool notify = true)
        {
            if (string.IsNullOrWhiteSpace(guid))
            {
                ClearPreview();
                return;
            }
            int index = GetAssetListItemIndex(guid);
            if (index < 0)
            {
                ClearPreview();
                return;
            }
            // 滚动
            m_AssetList.ScrollToItem(index);
            // 选中
            if (notify)
            {
                m_AssetList.SetSelection(new int[] { index });
            }
            else
            {
                m_AssetList.SetSelectionWithoutNotify(new int[] { index });
            }
        }

        /// <summary>
        /// 清除列表选中
        /// <param name="notify"></param>
        /// </summary>
        private void ClearAssetListSelection(bool notify = true)
        {
            if (notify)
            {
                m_AssetList.ClearSelection();
            }
            else
            {
                m_AssetList.SetSelectionWithoutNotify(new int[] { });
            }
        }

        /// <summary>
        /// 获取条目元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ListItem GetAssetListItem(int index)
        {
#if UNITY_2021_1_OR_NEWER
            return (ListItem)m_AssetList.GetRootElementForIndex(index);
#else
            Type type = m_AssetList.GetType();
            FieldInfo fieldInfo = type.GetField("m_ScrollView", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return null;
            }
            ScrollView scrollView = (ScrollView)fieldInfo.GetValue(m_AssetList);
            if (scrollView == null)
            {
                return null;
            }
            VisualElement[] elements = scrollView.Children().ToArray();
            foreach (VisualElement element in elements)
            {
                ListItem listItem = (ListItem)element;
                if (listItem.index == index)
                {
                    return listItem;
                }
            }
            return null;
#endif
        }

        /// <summary>
        /// 获取条目元素
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private ListItem GetAssetListItem(string guid)
        {
            int index = GetAssetListItemIndex(guid);
            return (index < 0 ? null : GetAssetListItem(index));
        }

        /// <summary>
        /// 获取当前选中的条目元素
        /// </summary>
        /// <returns></returns>
        private ListItem GetSelectedAssetListItem()
        {
            return GetAssetListItem(m_AssetList.selectedIndex);
        }

        /// <summary>
        /// 获取条目下标
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private int GetAssetListItemIndex(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
            {
                return -1;
            }
            return m_AssetListData.FindIndex(v => v.MatchGuid(guid));
        }

        /// <summary>
        /// 获取选中的条目信息
        /// </summary>
        /// <returns></returns>
        private ItemInfo[] GetSelectedItemInfos()
        {
            return m_AssetList.selectedItems.Select(o => (ItemInfo)o).ToArray();
        }

        /// <summary>
        /// 获取选中的条目GUID列表
        /// </summary>
        /// <returns></returns>
        private string[] GetSelectedItemGuids()
        {
            return m_AssetList.selectedItems.Select(o => ((ItemInfo)o).guid).ToArray();
        }

        #endregion

        #region ListItem

        /// <summary>
        /// 创建条目元素
        /// </summary>
        /// <returns></returns>
        private ListItem CreateListItem()
        {
            ListItem listItem = new ListItem();
            // 浮动按钮
            listItem.enableFloatButton = true;
            listItem.floatButton.SetIcon(PipiUtility.GetIcon("Record Off"));
            listItem.floatButton.tooltip = "Locate to asset";
            listItem.floatButtonClicked += OnListItemFloatButtonClicked;
            // 注册右键菜单
            listItem.AddManipulator(new ContextualMenuManipulator(ItemMenuBuilder));
            return listItem;
        }

        /// <summary>
        /// 条目浮动按钮点击回调
        /// </summary>
        /// <param name="listItem"></param>
        private void OnListItemFloatButtonClicked(ListItem listItem)
        {
            if (listItem.userData == null) return;
            PipiUtility.SelectAsset(listItem.userData.guid);
        }

        /// <summary>
        /// 绑定条目
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        private void BindListItem(VisualElement element, int index)
        {
            if (index >= m_AssetListData.Count)
            {
                element.RemoveFromHierarchy();
                return;
            }
            ItemInfo itemInfo = m_AssetListData[index];
            // 应用数据
            ListItem listItem = (ListItem)element;
            listItem.index = index;
            listItem.userData = itemInfo;
            // 名称和图标
            string assetPath = AssetDatabase.GUIDToAssetPath(itemInfo.guid);
            if (itemInfo.IsValid())
            {
                const string displayNameSuffix = "^";
                string displayName = itemInfo.displayName;
                displayName = (!string.IsNullOrWhiteSpace(displayName) ? $"{displayName}{displayNameSuffix}" : itemInfo.Name);
                listItem.SetText(displayName);
                listItem.SetIcon(AssetDatabase.GetCachedIcon(assetPath));
            }
            else
            {
                const string missingAssetName = "<Missing Asset>";
                listItem.SetText(missingAssetName);
                listItem.SetIcon(PipiUtility.GetAssetIcon(assetPath));
            }
            // 置顶
            listItem.SetTop(itemInfo.top);
            // 重命名
            listItem.renameCallback += OnListItemRenamed;
            // 拖拽
            listItem.dragged += OnListItemDragged;
            // 监听事件
            listItem.RegisterCallback<MouseDownEvent>(OnListItemMouseDown);
        }

        /// <summary>
        /// 条目重命名回调
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="newName"></param>
        private bool OnListItemRenamed(ListItem listItem, string newName)
        {
            ItemInfo itemInfo = listItem.userData;
            if (string.IsNullOrWhiteSpace(newName) || newName.Equals(itemInfo.AssetName))
            {
                ProjectPinBoardManager.RemoveDisplayName(itemInfo.guid);
            }
            else
            {
                ProjectPinBoardManager.SetDisplayName(itemInfo.guid, newName);
            }
            // 聚焦到列表
            m_AssetList.Focus();
            return true;
        }

        /// <summary>
        /// 条目拖拽回调
        /// </summary>
        /// <param name="listItem"></param>
        private void OnListItemDragged(ListItem listItem)
        {
            TriggerDragging();
        }

        /// <summary>
        /// 取消条目绑定
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        private void UnbindListItem(VisualElement element, int index)
        {
            ListItem listItem = (ListItem)element;
            // 恢复状态
            listItem.HideNameTextField();
            // 移除数据
            listItem.index = -1;
            listItem.userData = null;
            // 取消事件监听
            listItem.UnregisterCallback<MouseDownEvent>(OnListItemMouseDown);
        }

        /// <summary>
        /// 列表条目鼠标点击回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnListItemMouseDown(MouseDownEvent evt)
        {
            if (!(evt.target is ListItem listItem))
            {
                return;
            }
            // 鼠标右键
            if (evt.button == 1)
            {
                int[] selectedIndices = m_AssetList.selectedIndices.ToArray();
                // 在 Unity 2020 中选中条目后快速点击鼠标右键会出现显示异常：列表会自动选择上一次选中的条目
                // 所以在这里主动选择已选中的条目来覆盖异常情况
                m_AssetList.SetSelectionWithoutNotify(selectedIndices);
                // 已选中条目中不包含当前条目时，清除已选中条目并选中当前条目
                if (!selectedIndices.Contains(listItem.index))
                {
                    m_AssetList.SetSelection(listItem.index);
                }
            }
        }

        #endregion

        #region ListItem Menu

        /// <summary>
        /// 条目菜单名称
        /// </summary>
        private static class ListItemMenuItemName
        {
            public const string Select = "Select";
            public const string Open = "Open";
            public const string ShowInExplorer = "Show In Explorer";
            public const string Top = "Top";
            public const string UnTop = "Un-top";
            public const string RePin = "Re-pin (Update pin time)";
            public const string Unpin = "Unpin";
            public const string SetDisplayName = "Set Display Name";
            public const string SetDisplayNameToPath = "Set Display Name to Path";
            public const string RemoveDisplayName = "Remove Display Name";
            public const string SetTags = "Set Tag(s)";
            public const string RemoveTags = "Remove Tag(s)";
        }

        /// <summary>
        /// 创建条目菜单
        /// </summary>
        /// <param name="evt"></param>
        private void ItemMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            object listItem = evt.target;
            DropdownMenu menu = evt.menu;
            menu.AppendAction(ListItemMenuItemName.Select, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.Open, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.ShowInExplorer, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.RePin, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.Unpin, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.Top, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.UnTop, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.SetDisplayName, OnItemMenuAction, EnabledOnSingleSelection, listItem);
            menu.AppendAction(ListItemMenuItemName.SetDisplayNameToPath, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.RemoveDisplayName, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();
            menu.AppendAction(ListItemMenuItemName.SetTags, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendAction(ListItemMenuItemName.RemoveTags, OnItemMenuAction, AlwaysEnabled, listItem);
            menu.AppendSeparator();

            DropdownMenuAction.Status AlwaysEnabled(DropdownMenuAction a) => DropdownMenuAction.AlwaysEnabled(a);

            DropdownMenuAction.Status EnabledOnSingleSelection(DropdownMenuAction a)
            {
                return (m_AssetList.selectedItems.Count() == 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }
        }

        /// <summary>
        /// 条目菜单行为回调
        /// </summary>
        /// <param name="action"></param>
        private void OnItemMenuAction(DropdownMenuAction action)
        {
            if (!(action.userData is ListItem item))
            {
                return;
            }
            switch (action.name)
            {
                case ListItemMenuItemName.Select:
                {
                    ItemInfo itemInfo = item.userData;
                    PipiUtility.FocusOnAsset(itemInfo.guid);
                    break;
                }
                case ListItemMenuItemName.Open:
                {
                    ItemInfo itemInfo = item.userData;
                    PipiUtility.OpenAsset(itemInfo.guid);
                    break;
                }
                case ListItemMenuItemName.ShowInExplorer:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        PipiUtility.ShowInExplorer(itemInfo.guid);
                    }
                    break;
                }
                case ListItemMenuItemName.RePin:
                {
                    ProjectPinBoardManager.Pin(GetSelectedItemGuids());
                    break;
                }
                case ListItemMenuItemName.Unpin:
                {
                    ProjectPinBoardManager.Unpin(GetSelectedItemGuids());
                    break;
                }
                case ListItemMenuItemName.Top:
                {
                    ProjectPinBoardManager.Top(GetSelectedItemGuids(), true);
                    break;
                }
                case ListItemMenuItemName.UnTop:
                {
                    ProjectPinBoardManager.Top(GetSelectedItemGuids(), false);
                    break;
                }
                case ListItemMenuItemName.SetDisplayName:
                {
                    item.ShowNameTextField();
                    break;
                }
                case ListItemMenuItemName.SetDisplayNameToPath:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        string path = itemInfo.Path.Replace("Assets/", "");
                        ProjectPinBoardManager.SetDisplayName(itemInfo.guid, path);
                    }
                    break;
                }
                case ListItemMenuItemName.RemoveDisplayName:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        ProjectPinBoardManager.RemoveDisplayName(itemInfo.guid);
                    }
                    break;
                }
                case ListItemMenuItemName.SetTags:
                {
                    ShowTagPopup(GetSelectedItemInfos());
                    break;
                }
                case ListItemMenuItemName.RemoveTags:
                {
                    foreach (ItemInfo itemInfo in GetSelectedItemInfos())
                    {
                        ProjectPinBoardManager.RemoveTags(itemInfo.guid);
                    }
                    break;
                }
            }
        }

        #endregion

    }

}
