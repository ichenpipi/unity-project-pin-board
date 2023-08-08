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
        /// 工具栏文件夹置顶开关
        /// </summary>
        private ToolbarToggle m_ToolbarTopFolderToggle = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarTopFolderToggle()
        {
            m_ToolbarTopFolderToggle = new ToolbarToggle()
            {
                name = "FolderToggle",
                tooltip = "Keep folders on top",
                focusable = false,
                style =
                {
                    flexShrink = 0,
                    width = 25,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 2,
                    paddingRight = 2,
                    marginLeft = -1,
                    marginRight = 0,
                    left = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_Toolbar.Add(m_ToolbarTopFolderToggle);
            // 处理元素
            {
                VisualElement input = m_ToolbarTopFolderToggle.Q<VisualElement>("", "unity-toggle__input");
                input.style.flexGrow = 0;
            }
            // 图标
            m_ToolbarTopFolderToggle.Add(new Image()
            {
                image = PipiUtility.GetIcon("Folder Icon"),
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                }
            });
            // 回调
            m_ToolbarTopFolderToggle.RegisterValueChangedCallback(OnFolderToggleValueChanged);
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

    }

}
