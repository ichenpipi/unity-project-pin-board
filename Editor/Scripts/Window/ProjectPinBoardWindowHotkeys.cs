using System.Linq;
using UnityEditor;
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
                bool stopEvent = true;

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
                    Menu_Reload();
                }
                // Delete / Backspace
                else if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
                {
                    string[] names = GetSelectedItemInfos().Select(v => $"- {v.Name}").ToArray();
                    bool isOk = EditorUtility.DisplayDialog(
                        "[Project Pin Board] Unpin assets",
                        $"Are you sure to unpin the following assets?\n{string.Join("\n", names)}",
                        "Confirm!",
                        "Cancel"
                    );
                    if (isOk)
                    {
                        ProjectPinBoardManager.Unpin(GetSelectedItemGuids());
                    }
                }
                // 不响应
                else
                {
                    stopEvent = false;
                }

                if (stopEvent)
                {
                    // 阻止事件的默认行为，停止事件传播
                    evt.PreventDefault();
                    evt.StopImmediatePropagation();
                }
            });
        }

    }

}
