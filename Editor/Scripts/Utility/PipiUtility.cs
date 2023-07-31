using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 工具
    /// </summary>
    public static class PipiUtility
    {

        #region Asset Info Utility

        internal static Object GUIDToAsset(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return null;
            return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        }

        internal static bool IsValidAsset(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return (!string.IsNullOrEmpty(assetPath));
        }

        internal static bool IsFolder(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return false;
            return AssetDatabase.IsValidFolder(assetPath);
        }

        internal static string GetAssetName(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return "";
            return Path.GetFileNameWithoutExtension(assetPath);
        }

        internal static string GetAssetExtname(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return "";
            return Path.GetExtension(assetPath);
        }

        #endregion

        #region Editor Utility

        private static readonly string[] s_OpenInEditorIncludeList = new[]
        {
            ".unity", ".prefab", ".anim", ".controller", ".mixer",
            ".shadergraph",
        };

        private static readonly string[] s_OpenInScriptEditorIncludeList = new[]
        {
            ".cs", ".c", ".cpp", ".h", ".m",
            ".shader", ".cginc",
            ".uss", ".uxml",
            ".asmdef", ".asmref",
            ".txt", ".md", ".log", ".cfg", ".config",
            ".js", ".ts", ".lua",
            ".json", ".yaml", ".xml", ".xaml",
            ".css", ".html", ".vue", ".jsx",
        };

        /// <summary>
        /// 是否能在Unity编辑器中打开资源
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static bool CanOpenInEditor(string guid)
        {
            string extname = GetAssetExtname(guid);
            return s_OpenInEditorIncludeList.Contains(extname);
        }

        /// <summary>
        /// 是否能在脚本编辑器中打开资源
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static bool CanOpenInScriptEditor(string guid)
        {
            string extname = GetAssetExtname(guid);
            return s_OpenInScriptEditorIncludeList.Contains(extname);
        }

        /// <summary>
        /// 选中资源
        /// </summary>
        /// <param name="guid"></param>
        internal static void SelectAsset(string guid)
        {
            Object asset = GUIDToAsset(guid);
            if (!asset) return;

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        /// <summary>
        /// 高亮资源
        /// </summary>
        /// <param name="guid"></param>
        internal static void PingAsset(string guid)
        {
            Object asset = GUIDToAsset(guid);
            if (!asset) return;

            EditorUtility.FocusProjectWindow();

            EditorGUIUtility.PingObject(asset);
        }

        /// <summary>
        /// 聚焦到资源
        /// </summary>
        /// <param name="guid"></param>
        internal static void FocusOnAsset(string guid)
        {
            Object asset = GUIDToAsset(guid);
            if (!asset) return;

            EditorUtility.FocusProjectWindow();

            if (IsFolder(guid))
            {
                Selection.activeObject = null;
                EditorApplication.delayCall += () => ShowFolderContents(guid);
            }
            else
            {
                Selection.activeObject = asset;
                EditorApplication.delayCall += () => EditorGUIUtility.PingObject(asset);
            }
        }

        /// <summary>
        /// 打开资源
        /// </summary>
        /// <param name="guid"></param>
        internal static void OpenAsset(string guid)
        {
            Object asset = GUIDToAsset(guid);
            if (!asset) return;

            AssetDatabase.OpenAsset(asset);
        }

        /// <summary>
        /// 展示文件夹内容
        /// </summary>
        /// <param name="guid"></param>
        internal static void ShowFolderContents(string guid)
        {
            Object asset = GUIDToAsset(guid);
            if (!asset) return;

            Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            MethodInfo showFolderContentsMethod = projectBrowserType!.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo instanceField = projectBrowserType!.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public);

            object projectBrowser = instanceField!.GetValue(null);
            showFolderContentsMethod?.Invoke(projectBrowser, new object[] { asset.GetInstanceID(), true });
        }

        /// <summary>
        /// 编辑器主题背景颜色
        /// </summary>
        public static Color EditorBackgroundColor
        {
            get
            {
                // EditorGUIUtility.GetDefaultBackgroundColor
                float num = EditorGUIUtility.isProSkin ? 0.22f : 0.76f;
                return new Color(num, num, num, 1f);
            }
        }

        #endregion

        #region Editor Builtin Icon

        internal static Texture GetIcon(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName).image;
        }

        private static readonly Dictionary<string, string> s_AssetIconNameMap = new Dictionary<string, string>
        {
            { "", "d_DefaultAsset Icon" },

            { ".prefab", "d_Prefab Icon" },
            { ".scene", "d_SceneAsset Icon" },
            { ".asset", "d_ScriptableObject Icon" },
            { ".anim", "d_AnimationClip Icon" },
            { ".controller", "d_AnimatorController Icon" },

            { ".mat", "d_Material Icon" },
            { ".shader", "d_Shader Icon" },

            { ".uss", "UssScript Icon" },
            { ".uxml", "UxmlScript Icon" },

            { ".png", "d_RawImage Icon" },
            { ".jpg", "d_RawImage Icon" },
            { ".jpeg", "d_RawImage Icon" },

            { ".fbx", "d_PrefabVariant Icon" },

            { ".mp3", "d_AudioImporter Icon" },
            { ".aac", "d_AudioImporter Icon" },
        };

        internal static Texture GetAssetIcon(string path = "")
        {
            string extname = Path.GetExtension(path).ToLower();
            if (!s_AssetIconNameMap.TryGetValue(extname, out string iconName))
            {
                iconName = s_AssetIconNameMap[""];
            }
            return GetIcon(iconName);
        }

        #endregion

        #region OS Utility

        // public static void ShowInExplorer(string guid)
        // {
        //     string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        //     if (string.IsNullOrEmpty(assetPath)) return;
        //     EditorUtility.RevealInFinder(assetPath);
        // }

        // public static void ShowInExplorer(string path)
        // {
        //     #if UNITY_EDITOR_WIN
        //     System.Diagnostics.Process.Start("explorer.exe", path);
        //     #endif
        // }

        /// <summary>
        /// 在操作系统资源管理器中展示
        /// </summary>
        /// <param name="guid"></param>
        internal static void ShowInExplorer(string guid)
        {
            ShowInExplorer(GUIDToAsset(guid));
        }

        /// <summary>
        /// 在操作系统资源管理器中展示
        /// </summary>
        /// <param name="asset"></param>
        internal static void ShowInExplorer(Object asset)
        {
            if (asset == null) return;
            // 如果没有选中任何资源，就用编辑器开放的接口
            if (Selection.activeObject == null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                EditorUtility.RevealInFinder(assetPath);
                return;
            }
            // 保存当前的选择
            Object lastObject = Selection.activeObject;
            // 选择目标资源并执行资源菜单项
            Selection.activeObject = asset;
            Selection.activeInstanceID = asset.GetInstanceID();
            // 需要区分 macOS 和 Windows
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                EditorApplication.ExecuteMenuItem("Assets/Reveal in Finder");
            }
            else
            {
                EditorApplication.ExecuteMenuItem("Assets/Show in Explorer");
            }
            // 恢复之前的选择
            Selection.activeObject = lastObject;
        }

        /// <summary>
        /// 保存内容到系统剪切板
        /// </summary>
        /// <param name="text"></param>
        internal static void SaveToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        #endregion

        #region Other

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        internal static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 生成 GUID
        /// </summary>
        /// <returns></returns>
        internal static string NewGuid()
        {
            return Guid.NewGuid().ToString("D");
        }

        #endregion

        #region Local Storage

        /// <summary>
        /// 获取本地序列化的数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static T GetLocal<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                return new T();
            }
            string jsonString = File.ReadAllText(path, Encoding.UTF8);
            try
            {
                return JsonUtility.FromJson<T>(jsonString);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new T();
            }
        }

        /// <summary>
        /// 将数据序列化到本地
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        internal static void SetLocal<T>(string path, T value)
        {
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            string jsonString = JsonUtility.ToJson(value, true);
            File.WriteAllText(path, jsonString, Encoding.UTF8);
        }

        #endregion

    }

}
