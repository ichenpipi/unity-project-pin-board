using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
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
                    if (!string.IsNullOrWhiteSpace(m_FirstSelectedItemGuid))
                    {
                        ProjectPinBoardUtil.ShowInExplorer(m_FirstSelectedItemGuid);
                        evt.StopPropagation();
                    }
                }
                // Ctrl + F
                else if (evt.ctrlKey && evt.keyCode == KeyCode.F)
                {
                    FocusToSearchField();
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
