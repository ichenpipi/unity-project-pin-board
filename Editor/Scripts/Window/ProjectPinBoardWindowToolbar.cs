using System.Linq;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 工具栏
        /// </summary>
        private VisualElement m_Toolbar = null;

        /// <summary>
        /// 初始化
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

            // 分割线
            VisualElement separator = rootVisualElement.Q<VisualElement>("Separator");
            {
                separator.style.borderBottomColor = separatorColor;
            }

            // 搜索栏
            InitToolbarSearchField();
            // 过滤器
            InitToolbarFilterButton();
            // 置顶文件夹开关
            InitToolbarTopFolderToggle();
            // 预览开关
            InitToolbarPreviewToggle();
            // 同步选择开关
            InitToolbarSyncSelectionToggle();
            // 列表排序菜单
            InitToolbarSortingMenu();

            // 特殊处理最后一个元素的样式
            VisualElement[] elements = m_Toolbar.Children().ToArray();
            for (int i = 0; i < elements.Length; i++)
            {
                if (i == elements.Length - 1)
                {
                    VisualElement element = elements[i];
                    element.style.marginRight = -1;
                }
            }
        }

    }

}
