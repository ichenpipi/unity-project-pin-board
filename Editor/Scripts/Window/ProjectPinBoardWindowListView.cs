using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（内容列表）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 内容列表
        /// </summary>
        private ListView m_ListView = null;

        /// <summary>
        /// 初始化内容列表
        /// </summary>
        private void InitContentListView()
        {
            m_ListView = new ListView()
            {
                name = "ContentList",
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
            m_FirstSelectedGuid = first.guid;
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

        /// <summary>
        /// 更新内容列表
        /// </summary>
        private void UpdateContentListView()
        {
            // 取消选中
            m_ListView.ClearSelection();

            // 列表数据
            m_ListView.itemsSource = m_ListData;
#if UNITY_2021_2_OR_NEWER
            m_ListView.Rebuild();
#else
            m_ListView.Refresh();
#endif
        }

        /// <summary>
        /// 聚焦到列表
        /// </summary>
        public void FocusToContentListView()
        {
            m_ListView.Focus();
        }

    }

}
