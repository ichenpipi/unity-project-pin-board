using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

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
                    borderBottomColor = separatorColor,
                    marginTop = margin,
                    marginBottom = margin,
                    flexShrink = 0,
                },
            };
        }

    }

}
