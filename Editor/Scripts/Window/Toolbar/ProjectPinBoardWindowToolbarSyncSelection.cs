using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    ///  窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 工具栏同步选择开关
        /// </summary>
        private ToolbarToggle m_ToolbarSyncSelectionToggle = null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitToolbarSyncSelectionToggle()
        {
            m_ToolbarSyncSelectionToggle = new ToolbarToggle()
            {
                name = "SyncSelection",
                tooltip = "Sync selection to Project Browser",
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
            m_Toolbar.Add(m_ToolbarSyncSelectionToggle);
            // 处理元素
            {
                VisualElement input = m_ToolbarSyncSelectionToggle.Q<VisualElement>("", "unity-toggle__input");
                input.style.flexGrow = 0;
            }
            // 图标
            m_ToolbarSyncSelectionToggle.Add(new Image()
            {
                image = PipiUtility.GetIcon("d_Grid.Default"),
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                }
            });
            // 回调
            m_ToolbarSyncSelectionToggle.RegisterValueChangedCallback(OnSyncSelectionValueChanged);
        }

        /// <summary>
        /// 同步选择开关回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnSyncSelectionValueChanged(ChangeEvent<bool> evt)
        {
            ProjectPinBoardSettings.syncSelection = evt.newValue;
        }

    }

}
