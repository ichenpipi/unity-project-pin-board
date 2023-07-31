using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        /// <summary>
        /// 拖拽标签
        /// </summary>
        private const string k_DragAndDropGenericType = "ProjectPinBoard-ListItem";

        #region Dragging

        /// <summary>
        /// 触发拖拽
        /// </summary>
        private void TriggerDragging()
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(k_DragAndDropGenericType, this);
            DragAndDrop.StartDrag("Assets");

            List<Object> assets = new List<Object>();
            object[] itemInfos = m_ListView.selectedItems.ToArray();
            foreach (ItemInfo itemInfo in itemInfos)
            {
                Object asset = itemInfo.Asset;
                if (asset) assets.Add(asset);
            }
            DragAndDrop.objectReferences = assets.ToArray();
        }

        #endregion

        #region Drop Area

        /// <summary>
        /// 拖放区域
        /// </summary>
        private VisualElement m_DropArea = null;

        /// <summary>
        /// 拖放样式
        /// </summary>
        private VisualElement m_DropTip = null;

        /// <summary>
        /// 初始化拖放区域
        /// </summary>
        private void InitDropArea()
        {
            // 绑定视图
            m_DropArea = new VisualElement()
            {
                name = "DropArea",
                style =
                {
                    display = DisplayStyle.None,
                    position = Position.Absolute,
                    top = 0,
                    bottom = 0,
                    left = 0,
                    right = 0,
                    backgroundColor = new Color(0f, 0f, 0f, 80 / 255f),
                }
            };
            rootVisualElement.Add(m_DropArea);

            // 放置样式
            const int dropTipBorderWidth = 2;
            Color dropTipBorderColor = new Color(210 / 255f, 210 / 255f, 210 / 255f, 1f);
            float dropTipMarginTop = m_Toolbar.style.height.value.value;
            m_DropTip = new VisualElement()
            {
                name = "DropTip",
                style =
                {
                    display = DisplayStyle.None,
                    flexBasis = Length.Percent(100),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    borderTopWidth = dropTipBorderWidth,
                    borderBottomWidth = dropTipBorderWidth,
                    borderLeftWidth = dropTipBorderWidth,
                    borderRightWidth = dropTipBorderWidth,
                    borderTopColor = dropTipBorderColor,
                    borderBottomColor = dropTipBorderColor,
                    borderLeftColor = dropTipBorderColor,
                    borderRightColor = dropTipBorderColor,
                    marginTop = dropTipMarginTop,
                }
            };
            m_DropArea.Add(m_DropTip);
            // 文本
            Label label = new Label()
            {
                name = "Label",
                text = "Drop to Pin",
                style =
                {
                    paddingLeft = 0,
                    paddingRight = 0,
                    fontSize = 40,
                    color = new Color(210 / 255f, 210 / 255f, 210 / 255f, 1f),
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
#if UNITY_2021_1_OR_NEWER
                    unityTextOutlineColor = new Color(0f, 0f, 0f, 1f),
                    unityTextOutlineWidth = 1,
#endif
                }
            };
            m_DropTip.Add(label);

            // 监听拖拽事件
            m_DropArea.RegisterCallback<DragEnterEvent>(OnDragEnter);
            m_DropArea.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            m_DropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            m_DropArea.RegisterCallback<DragPerformEvent>(OnDragPerform);
            m_DropArea.RegisterCallback<DragExitedEvent>(OnDragExited);
            // 一些特殊处理
            {
                // If the mouse move quickly, DragExitedEvent will only be sent to panel.visualTree.
                // Register a callback there to get notified.
                rootVisualElement.panel?.visualTree.RegisterCallback<DragExitedEvent>(OnDragExited);

                // When opening the window, root.panel is not set yet. Use these callbacks to make
                // sure we register a DragExitedEvent callback on root.panel.visualTree.
                m_DropArea.RegisterCallback<AttachToPanelEvent>((evt) =>
                {
                    evt.destinationPanel.visualTree.RegisterCallback<DragExitedEvent>(OnDragExited);
                });
                m_DropArea.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    evt.originPanel.visualTree.UnregisterCallback<DragExitedEvent>(OnDragExited);
                });
            }

            // 根节点监听拖拽事件（用于启用/禁用拖放区域）
            rootVisualElement.RegisterCallback<DragEnterEvent>((evt) => EnableDropArea());
            rootVisualElement.RegisterCallback<DragLeaveEvent>((evt) => DisableDropArea());
            rootVisualElement.RegisterCallback<DragExitedEvent>((evt) => DisableDropArea());
        }

        /// <summary>
        /// 启用拖放区域
        /// </summary>
        private void EnableDropArea()
        {
            m_DropArea.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// 禁用拖放区域
        /// </summary>
        private void DisableDropArea()
        {
            m_DropArea.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 展示拖放样式
        /// </summary>
        private void ShowDropTip()
        {
            m_DropTip.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// 隐藏拖放样式
        /// </summary>
        private void HideDropTip()
        {
            m_DropTip.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 无效 GUID
        /// </summary>
        private const string k_InvalidGUID = "00000000000000000000000000000000";

        /// <summary>
        /// 是否可以放下
        /// </summary>
        /// <returns></returns>
        private bool CanDrop()
        {
            object genericData = DragAndDrop.GetGenericData(k_DragAndDropGenericType);
            if (genericData != null && (ProjectPinBoardWindow)genericData == this)
            {
                return false;
            }
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out long localId);
                if (string.IsNullOrEmpty(guid) || guid.Equals(k_InvalidGUID) || localId == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 拖拽进入元素回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragEnter(DragEnterEvent evt)
        {
            if (!CanDrop()) return;

            ShowDropTip();
        }

        /// <summary>
        /// 拖拽离开元素回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragLeave(DragLeaveEvent evt)
        {
            if (!CanDrop()) return;

            HideDropTip();
        }

        /// <summary>
        /// 拖拽结束回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragExited(DragExitedEvent evt)
        {
            if (!CanDrop()) return;

            HideDropTip();
        }

        /// <summary>
        /// 拖拽状态更新回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (CanDrop())
            {
                ShowDropTip();
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        /// <summary>
        /// 拖拽执行回调
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();

            if (!CanDrop()) return;

            foreach (Object obj in DragAndDrop.objectReferences)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out long localId);
                if (guid != null && localId != 0)
                {
                    ProjectPinBoardManager.Pin(guid);
                }
            }
        }

        #endregion

    }

}
