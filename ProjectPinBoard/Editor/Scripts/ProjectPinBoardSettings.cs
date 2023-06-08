using System;

namespace ChenPipi.ProjectPinBoard
{

    /// <summary>
    /// PinBoard 设置
    /// </summary>
    public static class ProjectPinBoardSettings
    {

        [Serializable]
        private class Settings
        {
            public int version = 0;
            public bool topFolder = true;
            public bool enablePreview = true;
            public float dragLinePos = 250f;
            public bool syncSelection = true;
        }

        private static Settings s_Settings;

        private static Settings settings => (s_Settings ??= GetLocal());

        /// <summary>
        /// 置顶文件夹
        /// </summary>
        public static bool topFolder
        {
            get => settings.topFolder;
            set
            {
                settings.topFolder = value;
                Save();
            }
        }

        /// <summary>
        /// 启用预览
        /// </summary>
        public static bool enablePreview
        {
            get => settings.enablePreview;
            set
            {
                settings.enablePreview = value;
                Save();
            }
        }

        /// <summary>
        /// 启用预览
        /// </summary>
        public static float dragLinePos
        {
            get => settings.dragLinePos;
            set
            {
                settings.dragLinePos = value;
                Save();
            }
        }

        /// <summary>
        /// 同步选择
        /// </summary>
        public static bool syncSelection
        {
            get => settings.syncSelection;
            set
            {
                settings.syncSelection = value;
                Save();
            }
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        public static void Save()
        {
            SetLocal(settings);
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public static void Reload()
        {
            s_Settings = GetLocal();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public static void Reset()
        {
            SetLocal(s_Settings = new Settings());
        }

        #region Serialization & Deserialization

        /// <summary>
        /// 本地序列化文件路径
        /// </summary>
        internal static readonly string SerializedFilePath = string.Format(ProjectPinBoardUtil.LocalFilePathTemplate, "settings");

        /// <summary>
        /// 获取本地序列化的设置
        /// </summary>
        /// <returns></returns>
        private static Settings GetLocal()
        {
            return ProjectPinBoardUtil.GetLocal<Settings>(SerializedFilePath);
        }

        /// <summary>
        /// 将设置序列化到本地
        /// </summary>
        /// <param name="value"></param>
        private static void SetLocal(Settings value)
        {
            ProjectPinBoardUtil.SetLocal(SerializedFilePath, value);
        }

        #endregion

    }

}
