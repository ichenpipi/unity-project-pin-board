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

            // 搜索栏
            InitToolbarSearch();
            // 过滤器
            InitToolbarFilter();
            // 置顶文件夹开关
            InitToolbarTopFolder();
            // 预览开关
            InitToolbarPreview();
            // 同步选择开关
            InitToolbarSyncSelection();
            // 列表排序菜单
            InitToolbarSort();
        }

    }

}
