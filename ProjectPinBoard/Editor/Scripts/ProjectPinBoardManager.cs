using System;
using System.Collections.Generic;
using UnityEditor;

namespace ChenPipi.ProjectPinBoard
{

    /// <summary>
    /// PinBoard 管理器
    /// </summary>
    public static class ProjectPinBoardManager
    {

        #region Window

        public static void Open()
        {
            if (ProjectPinBoardWindow.HasOpenInstances())
            {
                ProjectPinBoardWindow window = ProjectPinBoardWindow.GetInstance();
                window.Show(true);
            }
            else
            {
                ProjectPinBoardWindow.CreateInstance();
            }
        }

        #endregion

        #region Pin & Unpin

        /// <summary>
        /// Pin 事件
        /// </summary>
        public static event Action<string[]> pinned;

        /// <summary>
        /// Unpin 事件
        /// </summary>
        public static event Action<string[]> unpinned;

        /// <summary>
        /// Pin 资源
        /// </summary>
        /// <param name="guids"></param>
        public static void Pin(string[] guids)
        {
            foreach (string guid in guids)
            {
                ProjectPinBoardData.AddItem(guid);
            }
            // 保存到本地并通知更新
            SaveData();
            // 事件通知
            pinned?.Invoke(guids);
        }

        /// <summary>
        /// Pin 资源
        /// </summary>
        /// <param name="guid"></param>
        public static void Pin(string guid)
        {
            ProjectPinBoardData.AddItem(guid);
            // 保存到本地并通知更新
            SaveData();
            // 事件通知
            pinned?.Invoke(new[] { guid });
        }

        /// <summary>
        /// Unpin 资源
        /// </summary>
        /// <param name="guids"></param>
        public static void Unpin(string[] guids)
        {
            foreach (string guid in guids)
            {
                ProjectPinBoardData.RemoveItem(guid);
            }
            // 保存到本地并通知更新
            SaveData();
            // 事件通知
            unpinned?.Invoke(guids);
        }

        /// <summary>
        /// Unpin 资源
        /// </summary>
        /// <param name="guid"></param>
        public static void Unpin(string guid)
        {
            ProjectPinBoardData.RemoveItem(guid);
            // 保存到本地并通知更新
            SaveData();
            // 事件通知
            unpinned?.Invoke(new[] { guid });
        }

        /// <summary>
        /// 资源是否已被 Pin
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsPinned(string guid)
        {
            return ProjectPinBoardData.HasItem(guid);
        }

        #endregion

        #region Top

        /// <summary>
        /// 置顶资源
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="top"></param>
        public static void Top(string[] guids, bool top = true)
        {
            foreach (string guid in guids)
            {
                ItemInfo item = ProjectPinBoardData.GetItem(guid);
                if (item != null) item.top = top;
            }
            // 保存到本地并通知更新
            SaveData();
        }

        /// <summary>
        /// 置顶资源
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="top"></param>
        public static void Top(string guid, bool top = true)
        {
            ItemInfo item = ProjectPinBoardData.GetItem(guid);
            if (item != null) item.top = top;
            // 保存到本地并通知更新
            SaveData();
        }

        #endregion

        #region DisplayName

        /// <summary>
        /// 显示名称非法字符
        /// </summary>
        private static readonly char[] s_InvalidDisplayNameChars = new[] { '?', '<', '>', '\\', ':', '*', '|', '\"' };

        /// <summary>
        /// 设置显示名称
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDisplayName(string guid, string value)
        {
            ItemInfo item = ProjectPinBoardData.GetItem(guid);
            if (item == null)
            {
                return false;
            }
            // 禁止特殊字符
            if (value.IndexOfAny(s_InvalidDisplayNameChars) != -1)
            {
                EditorUtility.DisplayDialog(
                    "Invalid Name!",
                    $"A file name can't contain any of the following characters: {s_InvalidDisplayNameChars.Join("")}",
                    "OK"
                );
                return false;
            }
            // 更新数据
            item.displayName = value.Trim();
            // 保存到本地并通知更新
            SaveData();
            return true;
        }

        /// <summary>
        /// 清除显示名称
        /// </summary>
        /// <param name="guid"></param>
        public static void ClearDisplayName(string guid)
        {
            ItemInfo item = ProjectPinBoardData.GetItem(guid);
            if (item == null) return;
            item.displayName = string.Empty;
            // 保存到本地并通知更新
            SaveData();
        }

        #endregion

        #region Tags

        /// <summary>
        /// 标签非法字符
        /// </summary>
        private static readonly char[] s_InvalidTagNameChars = new[] { '/', '?', '<', '>', '\\', ':', '*', '|', '\"' };

        /// <summary>
        /// 标签是否合法
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="showTips"></param>
        /// <returns></returns>
        public static bool IsValidTag(string tag, bool showTips = true)
        {
            if (tag.IndexOfAny(s_InvalidTagNameChars) != -1)
            {
                if (showTips)
                {
                    EditorUtility.DisplayDialog(
                        "Invalid tag name!",
                        $"A tag name can't contain any of the following characters: {s_InvalidTagNameChars.Join()}",
                        "OK"
                    );
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="tags"></param>
        public static void SetTags(string guid, List<string> tags)
        {
            ItemInfo item = ProjectPinBoardData.GetItem(guid);
            if (item == null) return;
            item.tags.Clear();
            tags.ForEach(v => item.AddTag(v));
            // 保存到本地并通知更新
            SaveData();
        }

        /// <summary>
        /// 清除标签
        /// </summary>
        /// <param name="guid"></param>
        public static void ClearTags(string guid)
        {
            ItemInfo item = ProjectPinBoardData.GetItem(guid);
            if (item == null) return;
            item.tags.Clear();
            // 保存到本地并通知更新
            SaveData();
        }

        #endregion

        #region Data

        /// <summary>
        /// 数据更新
        /// </summary>
        public static event Action dataUpdated;

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="triggerUpdate"></param>
        private static void SaveData(bool triggerUpdate = true)
        {
            ProjectPinBoardData.Save();
            if (triggerUpdate) TriggerUpdate();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public static void ClearData()
        {
            ProjectPinBoardData.Reset();
            TriggerUpdate();
        }

        /// <summary>
        /// 重新加载数据
        /// </summary>
        public static void ReloadData()
        {
            ProjectPinBoardData.Reload();
            TriggerUpdate();
        }

        /// <summary>
        /// 通知更新
        /// </summary>
        private static void TriggerUpdate()
        {
            dataUpdated?.Invoke();
        }

        #endregion

        #region Settings

        /// <summary>
        /// 重置设置
        /// </summary>
        public static void ResetSettings()
        {
            ProjectPinBoardSettings.Reset();
        }

        /// <summary>
        /// 重新加载设置
        /// </summary>
        public static void ReloadSettings()
        {
            ProjectPinBoardSettings.Reload();
        }

        #endregion

    }

}
