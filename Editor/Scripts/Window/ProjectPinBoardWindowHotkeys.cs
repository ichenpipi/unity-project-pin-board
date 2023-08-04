using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口（快捷键）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 监听快捷键
        /// </summary>
        private void RegisterHotkeys()
        {
            rootVisualElement.RegisterCallback<KeyDownEvent>((evt) =>
            {
                // Alt + Shift + R
                if (evt.altKey && evt.shiftKey && evt.keyCode == KeyCode.R)
                {
                    if (!string.IsNullOrWhiteSpace(m_FirstSelectedItemGuid))
                    {
                        PipiUtility.ShowInExplorer(m_FirstSelectedItemGuid);
                    }
                }
                // Ctrl + F
                else if (evt.ctrlKey && evt.keyCode == KeyCode.F)
                {
                    FocusToSearchField();
                }
                // F2
                else if (evt.keyCode == KeyCode.F2)
                {
                    ListItem item = GetSelectedAssetListItem();
                    item?.ShowNameTextField();
                }
                // F5
                else if (evt.keyCode == KeyCode.F5)
                {
                    Refresh();
                }
                // 停止事件传播
                evt.PreventDefault();
                evt.StopImmediatePropagation();
            });
        }

    }

}
