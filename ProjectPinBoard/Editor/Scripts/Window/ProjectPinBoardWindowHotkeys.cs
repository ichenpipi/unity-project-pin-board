using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard
{

    /// <summary>
    /// PinBoard 窗口（快捷键）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 监听快捷键
        /// </summary>
        private void RegisterHotkeys()
        {
            rootVisualElement.parent.RegisterCallback<KeyDownEvent>((evt) =>
            {
                // Alt + Shift + R
                if (evt.altKey && evt.shiftKey && evt.keyCode == KeyCode.R)
                {
                    if (!string.IsNullOrWhiteSpace(m_FirstSelectedGuid))
                    {
                        ProjectPinBoardUtil.ShowInExplorer(m_FirstSelectedGuid);
                        evt.StopPropagation();
                    }
                }
                // Ctrl + F
                else if (evt.ctrlKey && evt.keyCode == KeyCode.F)
                {
                    FocusToToolbarSearchField();
                    evt.StopPropagation();
                }
                // F2
                else if (evt.keyCode == KeyCode.F2)
                {
                    ListItem item = GetSelectedListItem();
                    if (item != null)
                    {
                        item.ShowNameTextField();
                        evt.StopPropagation();
                    }
                }
                // F5
                else if (evt.keyCode == KeyCode.F5)
                {
                    Refresh();
                    evt.StopPropagation();
                }
            });
        }

    }

}
