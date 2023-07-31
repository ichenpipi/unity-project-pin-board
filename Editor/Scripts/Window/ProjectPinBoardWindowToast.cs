// using System.Threading;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace ChenPipi.ProjectPinBoard.Editor
// {
//
//     /// <summary>
//     /// 窗口
//     /// </summary>
//     public partial class ProjectPinBoardWindow
//     {
//
//         /// <summary>
//         /// Toast
//         /// </summary>
//         private VisualElement m_Toast = null;
//
//         /// <summary>
//         /// 展示 Toast
//         /// </summary>
//         /// <param name="content"></param>
//         /// <param name="duration"></param>
//         private void ShowToast(string content, float duration = 1)
//         {
//             if (m_Toast == null)
//             {
//                 const float padding = 10;
//                 const float borderRadius = 10;
//                 m_Toast = new VisualElement()
//                 {
//                     name = "Toast",
//                     pickingMode = PickingMode.Ignore,
//                     style =
//                     {
//                         display = DisplayStyle.None,
//                         position = Position.Absolute,
//                         paddingTop = padding,
//                         paddingBottom = padding,
//                         paddingLeft = padding,
//                         paddingRight = padding,
//                         borderTopLeftRadius = borderRadius,
//                         borderTopRightRadius = borderRadius,
//                         borderBottomLeftRadius = borderRadius,
//                         borderBottomRightRadius = borderRadius,
//                         backgroundColor = new Color(0f, 0f, 0f, 0.9f),
//                         alignItems = Align.Center,
//                         justifyContent = Justify.Center,
//                     }
//                 };
//                 rootVisualElement.Add(m_Toast);
//
//                 // 文本
//                 Label label = new Label()
//                 {
//                     name = "ToastText",
//                     pickingMode = PickingMode.Ignore,
//                     style =
//                     {
//                         paddingLeft = 0,
//                         paddingRight = 0,
//                         fontSize = 15,
//                         unityTextAlign = TextAnchor.MiddleCenter,
//                         whiteSpace = WhiteSpace.Normal,
//                     }
//                 };
//                 m_Toast.Add(label);
//             }
//
//             // 更新位置尺寸
//             void UpdateTransform()
//             {
//                 Rect rootBound = rootVisualElement.worldBound;
//                 const float minHeight = 50;
//                 const float top = 100;
//                 const float width = 250;
//                 float left = (rootBound.width / 2) - (width / 2f);
//                 m_Toast.style.width = width;
//                 m_Toast.style.minHeight = minHeight;
//                 m_Toast.style.top = top;
//                 m_Toast.style.left = left;
//             }
//
//             // 监听事件
//             rootVisualElement.RegisterCallback<GeometryChangedEvent>((evt) => UpdateTransform());
//             m_Toast.RegisterCallback<GeometryChangedEvent>((evt) => UpdateTransform());
//
//             // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//
//             // 展示
//             void Show()
//             {
//                 m_Toast.style.display = DisplayStyle.Flex;
//                 m_Toast.BringToFront();
//             }
//
//             // 隐藏
//             void Hide()
//             {
//                 m_Toast.style.display = DisplayStyle.None;
//             }
//
//             // 更新文本
//             void UpdateContentText(string text)
//             {
//                 Label label = m_Toast.Q<Label>("ToastText");
//                 label.text = text;
//             }
//
//             // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//
//             // 移除旧的延迟任务
//             if (m_Toast.userData != null)
//             {
//                 ((CancellationTokenSource)m_Toast.userData).Cancel();
//                 m_Toast.userData = null;
//             }
//
//             // 更新文本
//             UpdateContentText(content);
//
//             // 更新位置
//             UpdateTransform();
//
//             // 展示
//             Show();
//
//             // 延迟隐藏
//             int delay = (int)(duration * 1000);
//             CancellationTokenSource tokenSource = new CancellationTokenSource();
//             Task.Delay(delay, tokenSource.Token).ContinueWith((task) =>
//             {
//                 task.Dispose();
//                 m_Toast.userData = null;
//                 Hide();
//             }, tokenSource.Token);
//             m_Toast.userData = tokenSource;
//         }
//
//     }
//
// }
