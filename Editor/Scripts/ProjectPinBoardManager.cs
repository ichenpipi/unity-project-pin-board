using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 管理器
    /// </summary>
    public static class ProjectPinBoardManager
    {

        #region File Path

        /// <summary>
        /// 基础名称
        /// </summary>
        internal const string BaseName = "ProjectPinBoard";

        /// <summary>
        /// 项目路径
        /// </summary>
        private static readonly string s_ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));

        /// <summary>
        /// 用户设置路径
        /// </summary>
        private static readonly string s_UserSettingsPath = Path.Combine(s_ProjectPath, "UserSettings");

        /// <summary>
        /// 本地序列化文件路径模板
        /// </summary>
        internal static readonly string LocalFilePathTemplate = Path.GetFullPath(Path.Combine(s_UserSettingsPath, BaseName + ".{0}.json"));

        #endregion

        #region Window

        public static void Open(bool forceReopen = false)
        {
            if (!forceReopen && ProjectPinBoardWindow.HasOpenInstances())
            {
                ProjectPinBoardWindow window = ProjectPinBoardWindow.GetOpenedInstance();
                window.Show(true);
                window.Focus();
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
                    "[Project Pin Board] Invalid display name",
                    $"A display name can't contain any of the following characters: {s_InvalidDisplayNameChars.Join("")}",
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
        public static void RemoveDisplayName(string guid)
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
                        "[Project Pin Board] Invalid tag name",
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
        public static void RemoveTags(string guid)
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
        /// <param name="sendNotification"></param>
        private static void SaveData(bool sendNotification = true)
        {
            ProjectPinBoardData.Save();
            if (sendNotification)
            {
                Notify();
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public static void ClearData()
        {
            ProjectPinBoardData.Reset();
            Notify();
        }

        /// <summary>
        /// 重新加载数据
        /// </summary>
        public static void ReloadData()
        {
            ProjectPinBoardData.Reload();
            Notify();
        }

        /// <summary>
        /// 通知更新
        /// </summary>
        private static void Notify()
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
