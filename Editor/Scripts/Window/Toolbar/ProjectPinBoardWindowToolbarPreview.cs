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
        private void InitToolbarPreviewToggle()
        {
            m_ToolbarPreviewToggle = new ToolbarToggle()
            {
                name = "PreviewToggle",
                tooltip = "Enable preview panel",
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
            m_Toolbar.Add(m_ToolbarPreviewToggle);
            // 处理元素
            {
                VisualElement input = m_ToolbarPreviewToggle.Q<VisualElement>("", "unity-toggle__input");
                input.style.flexGrow = 0;
            }
            // 图标
            m_ToolbarPreviewToggle.Add(new Image()
            {
                image = PipiUtility.GetIcon("d_scenevis_visible_hover"),
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                }
            });
            // 回调
            m_ToolbarPreviewToggle.RegisterValueChangedCallback(OnPreviewToggleValueChanged);
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
