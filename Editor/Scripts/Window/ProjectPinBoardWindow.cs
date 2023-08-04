using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region Asset

        /// <summary>
        /// 视觉树资源
        /// </summary>
        [SerializeField]
        private VisualTreeAsset visualTree = null;

        #endregion

        #region Instance

        /// <summary>
        /// 窗口标题
        /// </summary>
        private const string k_Title = "Project Pin Board";

        /// <summary>
        /// 窗口图标
        /// </summary>
        private const string k_Icon = "d_Favorite";

        /// <summary>
        /// 是否有已打开的窗口实例
        /// </summary>
        /// <returns></returns>
        public static bool HasOpenInstances()
        {
            return HasOpenInstances<ProjectPinBoardWindow>();
        }

        /// <summary>
        /// 获取已打开的窗口实例
        /// </summary>
        /// <returns></returns>
        public static ProjectPinBoardWindow GetOpenedInstance()
        {
            return HasOpenInstances() ? GetWindow<ProjectPinBoardWindow>() : null;
        }

        /// <summary>
        /// 创建窗口实例
        /// </summary>
        /// <returns></returns>
        public static ProjectPinBoardWindow CreateInstance()
        {
            // 销毁已存在的实例
            ProjectPinBoardWindow window = GetOpenedInstance();
            if (window != null)
            {
                window.Close();
            }
            // 创建新的的实例
            window = CreateWindow<ProjectPinBoardWindow>();
            window.titleContent = new GUIContent()
            {
                text = k_Title,
                image = PipiUtility.GetIcon(k_Icon),
            };
            window.minSize = new Vector2(250f, 200f);
            window.SetSize(600, 500);
            window.SetCenter();
            return window;
        }

        /// <summary>
        /// 设置窗口尺寸
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetSize(int width, int height)
        {
            Rect pos = position;
            pos.width = width;
            pos.height = height;
            position = pos;
        }

        /// <summary>
        /// 使窗口居中（基于 Unity 编辑器主窗口）
        /// </summary>
        /// <param name="offsetX">水平偏移</param>
        /// <param name="offsetY">垂直偏移</param>
        public void SetCenter(int offsetX = 0, int offsetY = 0)
        {
            Rect mainWindowPos = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = position;
            float centerOffsetX = (mainWindowPos.width - pos.width) * 0.5f;
            float centerOffsetY = (mainWindowPos.height - pos.height) * 0.5f;
            pos.x = mainWindowPos.x + centerOffsetX + offsetX;
            pos.y = mainWindowPos.y + centerOffsetY + offsetY;
            position = pos;
        }

        /// <summary>
        /// 展示通知
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fadeoutWait"></param>
        private void ShowNotification(string content, double fadeoutWait = 1f)
        {
            ShowNotification(new GUIContent(content), fadeoutWait);
        }

        #endregion

        #region Lifecycle

        private void Awake()
        {
            ProjectPinBoardData.Reload();
            ProjectPinBoardSettings.Reload();
        }

        private void OnEnable()
        {
            ProjectPinBoardManager.dataUpdated += Refresh;
            ProjectPinBoardManager.pinned += OnPinned;

            EditorApplication.projectChanged += Refresh;
        }

        private void OnDisable()
        {
            ProjectPinBoardManager.dataUpdated -= Refresh;
            ProjectPinBoardManager.pinned -= OnPinned;

            EditorApplication.projectChanged -= Refresh;
        }

        private void CreateGUI()
        {
            // 构建视觉树
            visualTree.CloneTree(rootVisualElement);
            // 生成元素
            Init();
        }

        #endregion

        private void Init()
        {
            // 初始化工具栏
            InitToolbar();
            // 初始化内容
            InitContent();

            // 初始化拖放区域
            InitDropArea();

            // 监听快捷键
            RegisterHotkeys();

            // 刷新数据
            Refresh();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void Refresh()
        {
            if (!IsContentInited()) return;

            // 收集信息
            CollectInfo();

            // 更新内容
            UpdateContent();
        }

        #region Sorting

        /// <summary>
        /// 排序方式
        /// </summary>
        private enum Sorting
        {

            /// <summary>
            /// 名称递增
            /// </summary>
            NameUp = 1,

            /// <summary>
            /// 名称递减
            /// </summary>
            NameDown = 2,

            /// <summary>
            /// 时间递增
            /// </summary>
            TimeUp = 3,

            /// <summary>
            /// 时间递减
            /// </summary>
            TimeDown = 4,

        }

        /// <summary>
        /// 排序优先级
        /// </summary>
        private static class SortingPriority
        {
            public const int Directory = 20;
            public const int Top = 10;
            public const int Base = 0;
            public const int Invalid = -1;
        }

        /// <summary>
        /// 基础排序函数
        /// </summary>
        private static readonly Comparison<ItemInfo> s_BaseSortingComparer = (a, b) =>
        {
            int ap = SortingPriority.Base;
            int bp = SortingPriority.Base;
            // 是否置顶文件夹
            if (ProjectPinBoardSettings.topFolder)
            {
                if (a.IsDirectory()) ap += SortingPriority.Directory;
                if (b.IsDirectory()) bp += SortingPriority.Directory;
            }
            // 是否置顶
            if (a.top) ap += SortingPriority.Top;
            if (b.top) bp += SortingPriority.Top;
            // 是否为有效资源
            if (!a.IsValid()) ap += SortingPriority.Invalid;
            if (!b.IsValid()) bp += SortingPriority.Invalid;
            return bp - ap;
        };

        /// <summary>
        /// 排序函数
        /// </summary>
        private static readonly Dictionary<Sorting, Comparison<ItemInfo>> s_SortingComparers = new Dictionary<Sorting, Comparison<ItemInfo>>()
        {
            {
                Sorting.NameUp, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    return baseSorting != 0 ? baseSorting : string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
                }
            },
            {
                Sorting.NameDown, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return (-string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
                }
            },
            {
                Sorting.TimeUp, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return a.time.CompareTo(b.time);
                }
            },
            {
                Sorting.TimeDown, (a, b) =>
                {
                    int baseSorting = s_BaseSortingComparer(a, b);
                    if (baseSorting != 0) return baseSorting;
                    return (-a.time.CompareTo(b.time));
                }
            },
        };

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="list"></param>
        private void Sort(ref List<ItemInfo> list)
        {
            list.Sort(s_SortingComparers[m_Sorting]);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void Filter(ref List<ItemInfo> list)
        {
            // 移除空格
            string text = m_SearchText.Trim();

            // 匹配类型
            SetTypeFilter(string.Empty, false);
            if (text.Contains("type:"))
            {
                Match match = Regex.Match(text, k_TypeFilterPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    SetTypeFilter(match.Groups[2].Value, false);
                    if (!string.IsNullOrWhiteSpace(m_FilteringType))
                    {
                        list = list.FindAll(v => v.MatchType(m_FilteringType));
                    }
                    text = match.Groups[3].Value;
                }
            }

            // 匹配标签
            SetTagFilter(string.Empty, false);
            if (text.Contains("tag:"))
            {
                Match match = Regex.Match(text, k_TagFilterPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    SetTagFilter(match.Groups[2].Value, false);
                    if (!string.IsNullOrWhiteSpace(m_FilteringTag))
                    {
                        list = list.FindAll(v => v.MatchTag(m_FilteringTag));
                    }
                    text = match.Groups[3].Value;
                }
            }

            // 匹配名称
            if (!string.IsNullOrWhiteSpace(text))
            {
                string pattern = text.Trim().ToCharArray().Join(".*");
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                list = list.FindAll(v => regex.Match(v.AssetName).Success || regex.Match(v.displayName).Success);
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// 当前列表中的所有类型
        /// </summary>
        private readonly List<string> m_ItemTypeList = new List<string>();

        /// <summary>
        /// 当前列表中的所有标签
        /// </summary>
        private readonly List<string> m_ItemTagList = new List<string>();

        /// <summary>
        /// 收集资源信息
        /// </summary>
        private void CollectInfo()
        {
            m_ItemTypeList.Clear();
            m_ItemTagList.Clear();
            foreach (ItemInfo itemInfo in ProjectPinBoardData.items)
            {
                // 类型
                string type = itemInfo.Type;
                if (!string.IsNullOrEmpty(type) && !m_ItemTypeList.Contains(type))
                {
                    m_ItemTypeList.Add(type);
                }
                // 标签
                foreach (string tag in itemInfo.tags)
                {
                    if (!m_ItemTagList.Contains(tag)) m_ItemTagList.Add(tag);
                }
            }
            m_ItemTypeList.Sort((Comparison<string>)Comparison);
            m_ItemTagList.Sort((Comparison<string>)Comparison);

            int Comparison(string a, string b)
            {
                return string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Pin 回调
        /// </summary>
        /// <param name="guids"></param>
        private void OnPinned(string[] guids)
        {
            if (!IsContentInited()) return;

            if (guids.Length > 0)
            {
                SetAssetListSelection(guids[0]);
            }
        }

        #endregion

        #region Settings

        /// <summary>
        /// 应用设置
        /// </summary>
        private void ApplySettings()
        {
            if (!IsContentInited()) return;

            m_ToolbarTopFolderToggle.SetValueWithoutNotify(ProjectPinBoardSettings.topFolder);
            m_ToolbarPreviewToggle.SetValueWithoutNotify(ProjectPinBoardSettings.enablePreview);
            m_ToolbarSyncSelectionToggle.SetValueWithoutNotify(ProjectPinBoardSettings.syncSelection);
            m_ContentSplitView.fixedPaneInitialDimension = ProjectPinBoardSettings.dragLinePos;
        }

        #endregion

    }

}
