using System;
using System.Collections.Generic;
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
        [SerializeField] private VisualTreeAsset visualTree = null;

        #endregion

        #region Instance

        /// <summary>
        /// 窗口标题
        /// </summary>
        private const string k_Title = "Project Pin Board";

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
                image = PipiUtility.GetIcon("Favorite"),
            };
            window.minSize = new Vector2(100, 100);
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
            ProjectPinBoardManager.dataUpdated += RefreshData;
            ProjectPinBoardManager.pinned += OnPinned;

            EditorApplication.projectChanged += RefreshData;
        }

        private void OnDisable()
        {
            ProjectPinBoardManager.dataUpdated -= RefreshData;
            ProjectPinBoardManager.pinned -= OnPinned;

            EditorApplication.projectChanged -= RefreshData;
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
            RefreshData();

            // 应用设置
            ApplySettings();
        }

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
        /// 刷新数据
        /// </summary>
        private void RefreshData()
        {
            if (!IsContentInited()) return;

            // 收集信息
            CollectInfo();

            // 更新内容
            UpdateContent();
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

            ApplySettings_DragLine();
        }

        /// <summary>
        /// 应用拖拽线
        /// </summary>
        private void ApplySettings_DragLine()
        {
            if (!IsContentInited()) return;

            float rootWidth = rootVisualElement.worldBound.width;
            float leftPaneMinWidth = m_AssetList.style.minWidth.value.value;
            float rightPaneMinWidth = m_PreviewPane.style.minWidth.value.value;
            float dragLinePos = ProjectPinBoardSettings.dragLinePos;
            if (dragLinePos < leftPaneMinWidth || dragLinePos > rootWidth - rightPaneMinWidth)
            {
                dragLinePos = leftPaneMinWidth;
            }
            else
            {
                if (m_ContentSplitView.fixedPaneIndex == 1)
                {
                    dragLinePos = rootWidth - dragLinePos;
                }
            }
            m_ContentSplitView.fixedPaneInitialDimension = dragLinePos;
        }

        #endregion

    }

}
