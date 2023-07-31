using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 数据
    /// </summary>
    public static class ProjectPinBoardData
    {

        [Serializable]
        private class UserData
        {
            public int version = 0;
            public List<ItemInfo> items = new List<ItemInfo>();
        }

        private static UserData s_UserData;

        private static UserData userData
        {
            get
            {
                if (s_UserData == null)
                {
                    s_UserData = GetLocal();
                    GenerateMapping();
                }
                return s_UserData;
            }
        }

        #region Items

        public static List<ItemInfo> items => userData.items;

        private static readonly Dictionary<string, ItemInfo> s_Guid2Item = new Dictionary<string, ItemInfo>();

        private static void GenerateMapping()
        {
            s_Guid2Item.Clear();
            foreach (ItemInfo item in items)
            {
                s_Guid2Item.Add(item.guid, item);
            }
        }

        public static ItemInfo GetItem(string guid)
        {
            if (s_Guid2Item.TryGetValue(guid, out ItemInfo item))
            {
                return item;
            }
            return null;
        }

        public static void AddItem(string guid)
        {
            if (s_Guid2Item.TryGetValue(guid, out ItemInfo item))
            {
                item.time = PipiUtility.GetTimestamp();
                return;
            }

            item = new ItemInfo()
            {
                guid = guid,
                time = PipiUtility.GetTimestamp(),
            };
            items.Add(item);
            s_Guid2Item.Add(guid, item);
        }

        public static void RemoveItem(string guid)
        {
            if (s_Guid2Item.TryGetValue(guid, out ItemInfo item))
            {
                s_Guid2Item.Remove(guid);
                items.Remove(item);
            }
        }

        public static bool HasItem(string guid)
        {
            return s_Guid2Item.TryGetValue(guid, out ItemInfo _);
        }

        #endregion

        #region Basic Interface

        /// <summary>
        /// 保存到本地
        /// </summary>
        public static void Save()
        {
            SetLocal(userData);
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public static void Reload()
        {
            s_UserData = GetLocal();
            GenerateMapping();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public static void Reset()
        {
            s_Guid2Item.Clear();
            SetLocal(s_UserData = new UserData());
        }
        
        #endregion

        #region Serialization & Deserialization

        /// <summary>
        /// 本地序列化文件路径
        /// </summary>
        internal static readonly string SerializedFilePath = string.Format(ProjectPinBoardManager.LocalFilePathTemplate, "data");

        /// <summary>
        /// 获取本地序列化的数据
        /// </summary>
        /// <returns></returns>
        private static UserData GetLocal()
        {
            return PipiUtility.GetLocal<UserData>(SerializedFilePath);
        }

        /// <summary>
        /// 将数据序列化到本地
        /// </summary>
        /// <param name="value"></param>
        private static void SetLocal(UserData value)
        {
            PipiUtility.SetLocal(SerializedFilePath, value);
        }

        #endregion

    }

    #region ItemInfo

    /// <summary>
    /// PinBoard 条目信息
    /// </summary>
    [Serializable]
    public class ItemInfo
    {

        /// <summary>
        /// 资源 GUID
        /// </summary>
        public string guid = string.Empty;

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> tags = new List<string>();

        /// <summary>
        /// Pin 时间
        /// </summary>
        public long time = 0;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string displayName = string.Empty;

        /// <summary>
        /// 置顶
        /// </summary>
        public bool top = false;

        /// <summary>
        /// 资源
        /// </summary>
        public Object Asset => AssetDatabase.LoadAssetAtPath<Object>(Path);

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName => PipiUtility.GetAssetName(guid);

        /// <summary>
        /// 名称
        /// </summary>
        public string Name => (string.IsNullOrWhiteSpace(displayName) ? AssetName : displayName);

        /// <summary>
        /// 名称
        /// </summary>
        public string Type => (AssetDatabase.GetMainAssetTypeAtPath(Path)?.Name ?? string.Empty);

        /// <summary>
        /// 资源 AssetBundle
        /// </summary>
        public string AssetBundle => (IsValid() ? AssetDatabase.GetImplicitAssetBundleName(Path) : string.Empty);

        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path => AssetDatabase.GUIDToAssetPath(guid);

        /// <summary>
        /// 资源是否有效
        /// </summary>
        public bool IsValid()
        {
            return (this.Asset != null);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            this.tags.Add(tag);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag"></param>
        public bool RemoveTag(string tag)
        {
            return this.tags.Remove(tag);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        public void RemoveAllTags()
        {
            this.tags.Clear();
        }

        /// <summary>
        /// 匹配GUID
        /// </summary>
        public bool MatchGUID(string guid)
        {
            return this.guid.Equals(guid, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 匹配多个标签
        /// </summary>
        /// <param name="tags"></param>
        public bool MatchTags(string[] tags)
        {
            foreach (string tag in tags)
            {
                if (!this.MatchTag(tag)) return false;
            }
            return true;
        }

        /// <summary>
        /// 匹配标签
        /// </summary>
        /// <param name="tag"></param>
        public bool MatchTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return true;
            return this.tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 匹配类型
        /// </summary>
        /// <param name="type"></param>
        public bool MatchType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return true;
            return this.Type.Equals(type, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsDirectory()
        {
            return AssetDatabase.IsValidFolder(this.Path);
        }

    }

    #endregion

}
