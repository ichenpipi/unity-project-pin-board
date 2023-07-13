using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// PinBoard 窗口（内容预览）
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        #region Preview Initialization

        /// <summary>
        /// 预览面板
        /// </summary>
        private VisualElement m_PreviewPane = null;

        /// <summary>
        /// 预览占位
        /// </summary>
        private VisualElement m_PreviewPlaceholder = null;

        /// <summary>
        /// 预览滚动视图
        /// </summary>
        private ScrollView m_PreviewScrollView = null;

        /// <summary>
        /// 预览图标
        /// </summary>
        private Image m_PreviewIcon = null;

        /// <summary>
        /// 预览名称
        /// </summary>
        private Label m_PreviewName = null;

        /// <summary>
        /// 预览展示名称
        /// </summary>
        private Label m_PreviewDisplayName = null;

        /// <summary>
        /// 预览分割线
        /// </summary>
        private VisualElement m_PreviewSeparator = null;

        /// <summary>
        /// 预览 GUID
        /// </summary>
        private PreviewItem m_PreviewGuidItem = null;

        /// <summary>
        /// 预览类型
        /// </summary>
        private PreviewItem m_PreviewTypeItem = null;

        /// <summary>
        /// 预览 AssetBundle
        /// </summary>
        private PreviewItem m_PreviewAssetBundleItem = null;

        /// <summary>
        /// 预览 AssetBundle
        /// </summary>
        private PreviewItem m_PreviewPathItem = null;

        /// <summary>
        /// 预览标签容器
        /// </summary>
        private VisualElement m_PreviewTagContainer = null;

        /// <summary>
        /// 预览悬浮按钮容器
        /// </summary>
        private VisualElement m_PreviewFloatingButtonContainer = null;

        /// <summary>
        /// 初始化内容预览
        /// </summary>
        private void InitContentPreview()
        {
            // 预览面板
            m_PreviewPane = new VisualElement()
            {
                name = "ContentPreview",
                style =
                {
                    flexBasis = Length.Percent(100),
                    minWidth = 150,
                }
            };
            m_ContentSplitView.Add(m_PreviewPane);

            // 占位
            m_PreviewPlaceholder = new VisualElement()
            {
                name = "Placeholder",
                style =
                {
                    flexBasis = Length.Percent(100),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            m_PreviewPane.Add(m_PreviewPlaceholder);
            // 占位文本
            m_PreviewPlaceholder.Add(new Label()
            {
                text = "Preview",
                style =
                {
                    fontSize = 22,
                    color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 0.8f),
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            });

            // 预览元素滚动视图
            m_PreviewScrollView = new ScrollView()
            {
                name = "ScrollView",
                style =
                {
                    display = DisplayStyle.None,
                    flexBasis = Length.Percent(100),
                    paddingTop = 10,
                    paddingBottom = 0,
                    minWidth = 100,
                    flexGrow = 1,
                }
            };
            m_PreviewPane.Add(m_PreviewScrollView);
            m_PreviewScrollView.contentContainer.style.flexGrow = 1;

            // 预览图标
            m_PreviewIcon = new Image
            {
                name = "Icon",
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    alignSelf = Align.Center,
                    width = Length.Percent(100),
                    height = 100,
                    paddingLeft = 5,
                    paddingRight = 5,
                },
            };
            m_PreviewScrollView.Add(m_PreviewIcon);
            // 名称
            m_PreviewName = new Label()
            {
                name = "Name",
                style =
                {
                    minHeight = 30,
                    paddingLeft = 5,
                    paddingRight = 5,
                    marginTop = 5,
                    fontSize = 15,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                },
            };
            m_PreviewScrollView.Add(m_PreviewName);
            // 右键菜单
            m_PreviewName.AddManipulator(new ContextualMenuManipulator(CopyNameMenuBuilder));
            // 展示名称
            m_PreviewDisplayName = new Label()
            {
                name = "DisplayName",
                style =
                {
                    paddingLeft = 5,
                    paddingRight = 5,
                    marginTop = 0,
                    marginBottom = 5,
                    fontSize = 12,
                    color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 1f),
                    unityFontStyleAndWeight = FontStyle.Italic,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                },
            };
            m_PreviewScrollView.Add(m_PreviewDisplayName);

            // 分割线
            m_PreviewSeparator = GenHorizontalSeparator();
            m_PreviewScrollView.Add(m_PreviewSeparator);

            // GUID
            m_PreviewGuidItem = new PreviewItem()
            {
                name = "GUID",
            };
            m_PreviewScrollView.Add(m_PreviewGuidItem);
            // 右键菜单
            m_PreviewGuidItem.AddManipulator(new ContextualMenuManipulator(CopyGuidMenuBuilder));

            // 类型
            m_PreviewTypeItem = new PreviewItem()
            {
                name = "Type",
            };
            m_PreviewScrollView.Add(m_PreviewTypeItem);
            // 右键菜单
            m_PreviewTypeItem.AddManipulator(new ContextualMenuManipulator(CopyTypeMenuBuilder));

            // AssetBundle
            m_PreviewAssetBundleItem = new PreviewItem()
            {
                name = "AssetBundle",
            };
            m_PreviewScrollView.Add(m_PreviewAssetBundleItem);
            // 右键菜单
            m_PreviewAssetBundleItem.AddManipulator(new ContextualMenuManipulator(CopyAssetBundleMenuBuilder));

            // 路径
            m_PreviewPathItem = new PreviewItem()
            {
                name = "Path",
            };
            m_PreviewScrollView.Add(m_PreviewPathItem);
            // 右键菜单
            m_PreviewPathItem.AddManipulator(new ContextualMenuManipulator(CopyPathMenuBuilder));

            // 标签
            m_PreviewTagContainer = new VisualElement()
            {
                name = "Tags",
                style =
                {
                    minHeight = 25,
                    paddingLeft = 0,
                    paddingRight = 5,
                    marginTop = 5,
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                },
            };
            m_PreviewScrollView.Add(m_PreviewTagContainer);

            // 悬浮按钮
            m_PreviewFloatingButtonContainer = new VisualElement()
            {
                name = "FloatingButtons",
                style =
                {
                    display = DisplayStyle.None,
                    flexShrink = 0,
                    paddingTop = 5,
                    paddingBottom = 5,
                    paddingLeft = 5,
                    paddingRight = 5,
                    borderTopWidth = 1,
                    backgroundColor = ProjectPinBoardUtil.EditorBackgroundColor,
                    borderTopColor = m_SeparatorColor,
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                    alignItems = Align.Center,
                    justifyContent = Justify.FlexStart,
                },
            };
            m_PreviewPane.Add(m_PreviewFloatingButtonContainer);
            {
                // 选择按钮
                Button selectButton = new Button()
                {
                    name = "SelectButton",
                    text = "Select",
                    style =
                    {
                        height = 20,
                    }
                };
                m_PreviewFloatingButtonContainer.Add(selectButton);
                selectButton.clicked += OnPreviewSelectButtonClick;
                // 打开按钮
                Button openButton = new Button()
                {
                    name = "OpenButton",
                    text = "Open",
                    style =
                    {
                        height = 20,
                    }
                };
                m_PreviewFloatingButtonContainer.Add(openButton);
                openButton.clicked += OnPreviewOpenButtonClick;
                // 在资源管理器中展示
                Button showInExplorerButton = new Button()
                {
                    name = "ShowInExplorerButton",
                    text = "Show In Explorer",
                    style =
                    {
                        height = 20,
                    }
                };
                m_PreviewFloatingButtonContainer.Add(showInExplorerButton);
                showInExplorerButton.clicked += OnPreviewShowInExplorerButtonClick;
            }
        }

        /// <summary>
        /// 创建复制GUID菜单
        /// </summary>
        /// <param name="evt"></param>
        private void CopyGuidMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            if (!(m_PreviewPane.userData is ItemInfo itemInfo)) return;
            evt.menu.AppendAction("Copy GUID", CopyTextAction, DropdownMenuAction.AlwaysEnabled, itemInfo.guid);
        }

        /// <summary>
        /// 创建复制类型菜单
        /// </summary>
        /// <param name="evt"></param>
        private void CopyTypeMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            if (!(m_PreviewPane.userData is ItemInfo itemInfo)) return;
            evt.menu.AppendAction("Copy Type", CopyTextAction, DropdownMenuAction.AlwaysEnabled, itemInfo.Type);
        }

        /// <summary>
        /// 创建复制名称菜单
        /// </summary>
        /// <param name="evt"></param>
        private void CopyNameMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            if (!(m_PreviewPane.userData is ItemInfo itemInfo)) return;
            evt.menu.AppendAction("Copy Name", CopyTextAction, DropdownMenuAction.AlwaysEnabled, itemInfo.Name);
        }

        /// <summary>
        /// 创建复制 AssetBundle 菜单
        /// </summary>
        /// <param name="evt"></param>
        private void CopyAssetBundleMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            if (!(m_PreviewPane.userData is ItemInfo itemInfo)) return;
            evt.menu.AppendAction("Copy AssetBundle", CopyTextAction, DropdownMenuAction.AlwaysEnabled, itemInfo.AssetBundle);
        }

        /// <summary>
        /// 创建复制路径菜单
        /// </summary>
        /// <param name="evt"></param>
        private void CopyPathMenuBuilder(ContextualMenuPopulateEvent evt)
        {
            if (!(m_PreviewPane.userData is ItemInfo itemInfo)) return;
            evt.menu.AppendAction("Copy Path", CopyTextAction, DropdownMenuAction.AlwaysEnabled, itemInfo.Path);
        }

        /// <summary>
        /// 复制文本行为
        /// </summary>
        /// <param name="action"></param>
        private void CopyTextAction(DropdownMenuAction action)
        {
            ProjectPinBoardUtil.SaveToClipboard((string)action.userData);
        }

        /// <summary>
        /// “选择”按钮点击回调
        /// </summary>
        private void OnPreviewSelectButtonClick()
        {
            if (m_PreviewPane.userData == null) return;
            ItemInfo itemInfo = (ItemInfo)m_PreviewPane.userData;
            ProjectPinBoardUtil.FocusOnAsset(itemInfo.guid);
        }

        /// <summary>
        /// “打开”按钮点击回调
        /// </summary>
        private void OnPreviewOpenButtonClick()
        {
            if (m_PreviewPane.userData == null) return;
            ItemInfo itemInfo = (ItemInfo)m_PreviewPane.userData;
            ProjectPinBoardUtil.OpenAsset(itemInfo.guid);
        }

        /// <summary>
        /// “在资源管理器中展示”按钮点击回调
        /// </summary>
        private void OnPreviewShowInExplorerButtonClick()
        {
            if (m_PreviewPane.userData == null) return;
            ItemInfo itemInfo = (ItemInfo)m_PreviewPane.userData;
            ProjectPinBoardUtil.ShowInExplorer(itemInfo.guid);
        }

        #endregion

        #region Preview Interface

        /// <summary>
        /// 开关预览
        /// </summary>
        /// <param name="enable"></param>
        public void TogglePreview(bool enable)
        {
            if (enable)
            {
                // 检查预览区域宽度
                float rootWidth = rootVisualElement.worldBound.width;
                float previewMinWidth = m_PreviewPane.style.minWidth.value.value;
                if (ProjectPinBoardSettings.dragLinePos > rootWidth - previewMinWidth)
                {
                    m_ContentSplitView.fixedPaneInitialDimension = rootWidth - previewMinWidth;
                }
                else
                {
                    m_ContentSplitView.fixedPaneInitialDimension = ProjectPinBoardSettings.dragLinePos;
                }
                // 取消折叠
                m_ContentSplitView.UnCollapse();
            }
            else
            {
                // 折叠预览区域
                m_ContentSplitView.CollapseChild(1);
            }
        }

        /// <summary>
        /// 清除预览
        /// </summary>
        public void ClearPreview()
        {
            m_PreviewPane.userData = null;

            m_PreviewPlaceholder.style.display = DisplayStyle.Flex;
            m_PreviewScrollView.style.display = DisplayStyle.None;

            m_PreviewIcon.image = null;
            m_PreviewName.text = string.Empty;
            m_PreviewDisplayName.text = string.Empty;
            m_PreviewDisplayName.style.display = DisplayStyle.None;

            m_PreviewGuidItem.titleLabel.text = string.Empty;
            m_PreviewGuidItem.contentLabel.text = string.Empty;

            m_PreviewTypeItem.titleLabel.text = string.Empty;
            m_PreviewTypeItem.contentLabel.text = string.Empty;

            m_PreviewAssetBundleItem.titleLabel.text = string.Empty;
            m_PreviewAssetBundleItem.contentLabel.text = string.Empty;

            m_PreviewPathItem.titleLabel.text = string.Empty;
            m_PreviewPathItem.contentLabel.text = string.Empty;

            m_PreviewTagContainer.Clear();

            m_PreviewFloatingButtonContainer.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 设置预览
        /// </summary>
        /// <param name="itemInfo"></param>
        private void SetPreview(ItemInfo itemInfo)
        {
            if (itemInfo == null)
            {
                ClearPreview();
                return;
            }

            // 保存数据
            m_PreviewPane.userData = itemInfo;
            // 隐藏占位，展示滚动视图
            m_PreviewPlaceholder.style.display = DisplayStyle.None;
            m_PreviewScrollView.style.display = DisplayStyle.Flex;

            // 资源信息
            string path = itemInfo.Path;
            Object asset = itemInfo.Asset;
            // 图标
            if (asset is Texture texture)
            {
                m_PreviewIcon.image = texture;
            }
            else
            {
                m_PreviewIcon.image = (asset ? AssetDatabase.GetCachedIcon(path) : ProjectPinBoardUtil.GetAssetIcon(path));
            }
            // 名称
            m_PreviewName.text = (asset ? asset.name : "<Missing Asset>");
            // 展示名称
            string displayName = itemInfo.displayName;
            m_PreviewDisplayName.style.display = (string.IsNullOrWhiteSpace(displayName) ? DisplayStyle.None : DisplayStyle.Flex);
            if (!string.IsNullOrWhiteSpace(displayName)) m_PreviewDisplayName.text = displayName;

            // GUID
            m_PreviewGuidItem.titleLabel.text = "GUID:";
            m_PreviewGuidItem.contentLabel.text = itemInfo.guid;
            // 类型
            m_PreviewTypeItem.titleLabel.text = "Type:";
            m_PreviewTypeItem.contentLabel.text = itemInfo.Type;
            // AssetBundle
            m_PreviewAssetBundleItem.titleLabel.text = "AssetBundle:";
            m_PreviewAssetBundleItem.contentLabel.text = (asset ? (!string.IsNullOrEmpty(itemInfo.AssetBundle) ? itemInfo.AssetBundle : "<None>") : string.Empty);
            // 路径
            m_PreviewPathItem.titleLabel.text = "Path:";
            m_PreviewPathItem.contentLabel.text = path;
            // 标签
            m_PreviewTagContainer.Clear();
            foreach (string tag in itemInfo.tags)
            {
                m_PreviewTagContainer.Add(GenTagLabel(tag));
            }

            // 悬浮按钮
            m_PreviewFloatingButtonContainer.style.display = DisplayStyle.Flex;
        }

        #endregion

        #region PreviewItem

        /// <summary>
        /// 预览条目
        /// </summary>
        private class PreviewItem : VisualElement
        {

            /// <summary>
            /// 标题标签
            /// </summary>
            public readonly Label titleLabel = null;

            /// <summary>
            /// 内容标签
            /// </summary>
            public readonly Label contentLabel = null;

            public PreviewItem(string title = "", string content = "")
            {
                // 自身样式
                this.style.paddingLeft = 5;
                this.style.paddingRight = 5;
                this.style.marginTop = 5;
                this.style.flexDirection = FlexDirection.Row;
                // 标题
                titleLabel = new Label()
                {
                    text = title,
                    style =
                    {
                        paddingLeft = 0,
                        paddingRight = 0,
                        marginRight = 5,
                        fontSize = 11,
                        unityFontStyleAndWeight = FontStyle.Normal,
                        unityTextAlign = TextAnchor.UpperLeft,
                        whiteSpace = WhiteSpace.Normal,
                    },
                };
                this.Add(titleLabel);
                // 内容
                contentLabel = new Label()
                {
                    text = content,
                    style =
                    {
                        paddingLeft = 0,
                        paddingRight = 0,
                        flexShrink = 1,
                        fontSize = 11,
                        unityFontStyleAndWeight = FontStyle.Bold,
                        unityTextAlign = TextAnchor.UpperLeft,
                        whiteSpace = WhiteSpace.Normal,
                    },
                };
                this.Add(contentLabel);
            }

        }

        #endregion

        #region TagLabel

        /// <summary>
        /// 标签边框圆角半径
        /// </summary>
        private const int k_TagBorderRadius = 9;

        /// <summary>
        /// 生成标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="clickCallback"></param>
        /// <returns></returns>
        private Label GenTagLabel(string text, Action clickCallback = null)
        {
            Label label = new Label()
            {
                name = $"Tag:{text}",
                text = text,
                style =
                {
                    alignSelf = Align.Center,
                    flexShrink = 1,
                    minWidth = 20,
                    minHeight = 20,
                    borderTopLeftRadius = k_TagBorderRadius,
                    borderTopRightRadius = k_TagBorderRadius,
                    borderBottomLeftRadius = k_TagBorderRadius,
                    borderBottomRightRadius = k_TagBorderRadius,
                    paddingLeft = 5,
                    paddingRight = 5,
                    marginLeft = 5,
                    marginTop = 5,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    whiteSpace = WhiteSpace.Normal,
                }
            };
            // 使用 uss 指定部分样式，代码目前无法指定 hover 状态样式
            label.AddToClassList("Tag");
            // 点击回调
            if (clickCallback != null)
            {
                label.RegisterCallback<MouseDownEvent>(_ => clickCallback());
            }
            return label;
        }

        #endregion

        #region Separator

        /// <summary>
        /// 分割线颜色
        /// </summary>
        private readonly Color m_SeparatorColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1f);

        /// <summary>
        /// 生成水平分割线
        /// </summary>
        /// <param name="margin"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private VisualElement GenHorizontalSeparator(float margin = 5, string name = "Separator")
        {
            return new VisualElement()
            {
                name = name,
                style =
                {
                    height = 1,
                    borderBottomWidth = 1,
                    borderBottomColor = m_SeparatorColor,
                    marginTop = margin,
                    marginBottom = margin,
                    flexShrink = 0,
                },
            };
        }

        #endregion

    }

}
