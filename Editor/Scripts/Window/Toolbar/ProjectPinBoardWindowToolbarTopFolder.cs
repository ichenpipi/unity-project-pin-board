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
        private void InitToolbarTopFolder()
        {
            m_ToolbarTopFolderToggle = new ToolbarToggle()
            {
                name = "FolderToggle",
                tooltip = "Keep folders on top",
                value = ProjectPinBoardSettings.topFolder,
                style =
                {
                    width = 25,
                    minWidth = 25,
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
                image = PipiUtility.GetIcon("d_Folder Icon"),
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
