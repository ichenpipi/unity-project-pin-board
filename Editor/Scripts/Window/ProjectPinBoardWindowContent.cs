using System.Collections.Generic;
#if !UNITY_2021_1_OR_NEWER
using System.Linq;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（内容）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 内容
        /// </summary>
        private VisualElement m_Content = null;

        /// <summary>
        /// 内容占位
        /// </summary>
        private VisualElement m_ContentPlaceholder = null;

        /// <summary>
        /// 内容分栏面板
        /// </summary>
        private TwoPaneSplitView m_ContentSplitView = null;

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitContent()
        {
            // 绑定视图
            m_Content = rootVisualElement.Q<VisualElement>("Content");
            {
                m_Content.style.flexBasis = Length.Percent(100);
                m_Content.style.marginTop = 0;
            }

            // 占位
            m_ContentPlaceholder = new VisualElement()
            {
                name = "Placeholder",
                style =
                {
                    flexBasis = Length.Percent(100),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_Content.Add(m_ContentPlaceholder);
            {
                // 占位文本
                m_ContentPlaceholder.Add(new Label()
                {
                    text = "Empty...",
                    style =
                    {
                        fontSize = 22,
                        color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 0.8f),
                        unityFontStyleAndWeight = FontStyle.Bold,
                    }
                });
            }

            // 内容分栏
            m_ContentSplitView = new TwoPaneSplitView()
            {
                name = "ContentSplitView",
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = ProjectPinBoardSettings.dragLinePos,
                orientation = TwoPaneSplitViewOrientation.Horizontal,
                style =
                {
                    flexBasis = Length.Percent(100),
                }
            };
            m_Content.Add(m_ContentSplitView);

            // 元素就绪后恢复预览区域状态
            {
                void Callback(GeometryChangedEvent evt)
                {
                    TogglePreview(ProjectPinBoardSettings.enablePreview);
                    m_ContentSplitView.UnregisterCallback<GeometryChangedEvent>(Callback);
                }

                m_ContentSplitView.RegisterCallback<GeometryChangedEvent>(Callback);
            }

            // 拖拽线
            {
                VisualElement dragLine = m_ContentSplitView.Q<VisualElement>("unity-dragline-anchor");
                IStyle style = dragLine.style;
                // 禁止拖拽线在Hover的时候变颜色
                dragLine.RegisterCallback<MouseEnterEvent>((evt) => style.backgroundColor = new Color(89 / 255f, 89 / 255f, 89 / 255f, 1f));
                // 拖动拖拽线后保存其位置
                dragLine.RegisterCallback<MouseUpEvent>((evt) => ProjectPinBoardSettings.dragLinePos = style.left.value.value);
            }

            // 初始化列表
            InitContentListView();
            // 初始化预览
            InitContentPreview();
        }

        /// <summary>
        /// 内容是否初始化
        /// </summary>
        /// <returns></returns>
        private bool IsContentInited()
        {
            return (m_Content != null);
        }

        /// <summary>
        /// 当前列表数据
        /// </summary>
        private List<ItemInfo> m_ListData = new List<ItemInfo>();

        /// <summary>
        /// 当前选中的资源 GUID
        /// </summary>
        private string m_FirstSelectedGuid = null;

        /// <summary>
        /// 更新内容
        /// </summary>
        private void UpdateContent()
        {
            if (!IsContentInited()) return;

            // 克隆数据副本
            m_ListData.Clear();
            m_ListData.AddRange(ProjectPinBoardData.items);

            // 处理数据
            if (m_ListData.Count > 0)
            {
                // 过滤
                Filter(ref m_ListData);
                // 排序
                Sort(ref m_ListData);
            }

            // 列表为空时展示占位
            m_ContentPlaceholder.style.display = (m_ListData.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None);
            m_ContentSplitView.style.display = (m_ListData.Count == 0 ? DisplayStyle.None : DisplayStyle.Flex);

            // 更新内容列表
            UpdateContentListView();

            // 恢复选中
            SelectListItem(m_FirstSelectedGuid);
        }

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
                int index = m_ListData.FindIndex(v => v.MatchGUID(guid));
                if (index >= 0)
                {
                    m_ListView.selectedIndex = index;
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

    }

}
