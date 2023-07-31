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
        /// 工具栏预览开关
        /// </summary>
        private ToolbarToggle m_ToolbarPreviewToggle = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarPreview()
        {
            // 预览开关
            {
                m_ToolbarPreviewToggle = new ToolbarToggle()
                {
                    name = "PreviewToggle",
                    tooltip = "Enable preview panel",
                    value = ProjectPinBoardSettings.enablePreview,
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
                m_Toolbar.Add(m_ToolbarPreviewToggle);
                // 图标
                m_ToolbarPreviewToggle.Add(new Image()
                {
                    image = PipiUtility.GetIcon("d_scenevis_visible_hover"),
                    scaleMode = ScaleMode.ScaleToFit,
                });
                // 回调
                m_ToolbarPreviewToggle.RegisterValueChangedCallback(OnPreviewToggleValueChanged);
            }
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

    }

}
