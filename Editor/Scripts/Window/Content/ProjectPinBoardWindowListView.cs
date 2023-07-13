using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（列表）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region ListView Initialization

        /// <summary>
        /// 列表
        /// </summary>
        private ListView m_ListView = null;

        /// <summary>
        /// 当前选中的第一个条目
        /// </summary>
        private string m_FirstSelectedItemGuid = null;

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitListView()
        {
            m_ListView = new ListView()
            {
                name = "ListView",
#if UNITY_2021_1_OR_NEWER
                fixedItemHeight = 18,
#else
                itemHeight = 18,
#endif
                selectionType = SelectionType.Multiple,
                makeItem = CreateItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                style =
                {
                    flexBasis = Length.Percent(100),
                    minWidth = 150,
                }
            };
            m_ContentSplitView.Add(m_ListView);

            // 列表选择变化回调
            m_ListView.onSelectionChange += OnListViewSelectionChange;
            // 列表条目选择（双击）回调
            m_ListView.onItemsChosen += OnListViewItemsChosen;
        }

        /// <summary>
        /// 列表选择变化回调
        /// </summary>
        /// <param name="objs"></param>
        private void OnListViewSelectionChange(IEnumerable<object> objs)
        {
            object[] infos = objs as object[] ?? objs.ToArray();
            if (!infos.Any())
            {
                // 清除预览
                ClearPreview();
                return;
            }
            // 缓存首个选择，用于热重载后恢复选中
            ItemInfo first = (ItemInfo)m_ListView.selectedItems.First();
            m_FirstSelectedItemGuid = first.guid;
            // 设置预览
            SetPreview(first);
            // 同步选择
            if (ProjectPinBoardSettings.syncSelection)
            {
                ProjectPinBoardUtil.SelectAsset(first.guid);
                // 夺回焦点
                this.Focus();
            }
        }

        /// <summary>
        /// 列表条目选择（双击）回调
        /// </summary>
        /// <param name="objs"></param>
        private void OnListViewItemsChosen(IEnumerable<object> objs)
        {
            object[] infos = objs as object[] ?? objs.ToArray();
            if (!infos.Any())
            {
                return;
            }
            ItemInfo info = (ItemInfo)infos.First();
            string guid = info.guid;
            // 是否可以打开
            if (ProjectPinBoardUtil.CanOpenInEditor(guid) || ProjectPinBoardUtil.CanOpenInScriptEditor(guid))
            {
                // 打开资源
                ProjectPinBoardUtil.OpenAsset(guid);
            }
            else
            {
                // 选中资源
                ProjectPinBoardUtil.FocusOnAsset(guid);
            }
        }

        #endregion

        #region ListView Interface

        /// <summary>
        /// 当前列表数据
        /// </summary>
        private List<ItemInfo> m_ListViewData = new List<ItemInfo>();

        /// <summary>
        /// 更新列表
        /// </summary>
        private void UpdateListView()
        {
            // 克隆数据副本
            m_ListViewData.Clear();
            m_ListViewData.AddRange(ProjectPinBoardData.items);

            // 处理数据
            if (m_ListViewData.Count > 0)
            {
                // 过滤
                Filter(ref m_ListViewData);
                // 排序
                Sort(ref m_ListViewData);
            }

            // 列表数据
            m_ListView.itemsSource = m_ListViewData;
#if UNITY_2021_2_OR_NEWER
            m_ListView.Rebuild();
#else
            m_ListView.Refresh();
#endif
        }

        /// <summary>
        /// 聚焦到列表
        /// </summary>
        public void FocusToListView()
        {
            m_ListView.Focus();
        }

        /// <summary>
        /// 清除列表选中
        /// </summary>
        private void ClearListViewSelection()
        {
            m_ListView.ClearSelection();
        }

        #endregion

        #region ListItem Lifecycle

        /// <summary>
        /// 创建条目元素
        /// </summary>
        /// <returns></returns>
        private ListItem CreateItem()
        {
            ListItem item = new ListItem();
            // 注册右键菜单
            item.AddManipulator(new ContextualMenuManipulator(ItemMenuBuilder));
            return item;
        }

        /// <summary>
        /// 绑定条目
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        private void BindItem(VisualElement element, int index)
        {
            if (index >= m_ListViewData.Count)
            {
                element.RemoveFromHierarchy();
                return;
            }
            // 应用数据
            ListItem listItem = (ListItem)element;
            listItem.index = index;
            listItem.Set(m_ListViewData[index]);
            // 监听事件
            listItem.RegisterCallback<MouseDownEvent>(OnListItemMouseDown);
        }

        /// <summary>
        /// 取消条目绑定
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        private void UnbindItem(VisualElement element, int index)
        {
            ListItem listItem = (ListItem)element;
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
                int[] selectedIndices = m_ListView.selectedIndices.ToArray();
                // 在 Unity 2020 中选中条目后快速点击鼠标右键会出现显示异常：列表会自动选择上一次选中的条目
                // 所以在这里主动选择已选中的条目来覆盖异常情况
                m_ListView.SetSelectionWithoutNotify(selectedIndices);
                // 已选中条目中不包含当前条目时，清除已选中条目并选中当前条目
                if (!selectedIndices.Contains(listItem.index))
                {
                    m_ListView.SetSelection(listItem.index);
                }
            }
        }

        #endregion

        #region ListItem Interface

        /// <summary>
        /// 获取条目元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ListItem GetListItem(int index)
        {
#if UNITY_2021_1_OR_NEWER
            return (ListItem)m_ListView.GetRootElementForIndex(index);
#else
            VisualElement[] elements = m_ListView.Children().ToArray();
            if (index < elements.Length)
            {
                return (ListItem)elements[index];
            }
            return null;
#endif
        }

        /// <summary>
        /// 获取当前选中的条目元素
        /// </summary>
        /// <returns></returns>
        private ListItem GetSelectedListItem()
        {
            return GetListItem(m_ListView.selectedIndex);
        }

        /// <summary>
        /// 选中条目
        /// </summary>
        /// <param name="guid"></param>
        public void SelectListItem(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                int index = m_ListViewData.FindIndex(v => v.MatchGUID(guid));
                if (index >= 0)
                {
                    m_ListView.SetSelection(index);
                    m_ListView.ScrollToItem(index);
                }
                else
                {
                    ClearPreview();
                }
            }
            else
            {
                ClearPreview();
            }
        }

        /// <summary>
        /// 获取选中的条目信息
        /// </summary>
        /// <returns></returns>
        private ItemInfo[] GetSelectedItemInfos()
        {
            return m_ListView.selectedItems.Select(o => (ItemInfo)o).ToArray();
        }

        /// <summary>
        /// 获取选中的条目GUID列表
        /// </summary>
        /// <returns></returns>
        private string[] GetSelectedItemGUIDs()
        {
            return m_ListView.selectedItems.Select(o => ((ItemInfo)o).guid).ToArray();
        }

        #endregion

    }

}
