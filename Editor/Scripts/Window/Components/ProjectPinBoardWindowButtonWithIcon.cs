using UnityEngine;
using UnityEngine.UIElements;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        private class ButtonWithIcon : Button
        {

            public readonly Image iconImage = new Image()
            {
                scaleMode = ScaleMode.ScaleToFit,
            };

            public readonly Label textLabel = new Label()
            {
                style =
                {
                    paddingTop = 1,
                    paddingBottom = 1,
                    paddingLeft = 2,
                    paddingRight = 2,
                    fontSize = 12,
                    unityFontStyleAndWeight = FontStyle.Normal,
                }
            };

            public ButtonWithIcon(string text = "", Texture icon = null)
            {
                // 自身样式
                this.style.flexDirection = FlexDirection.Row;
                this.style.alignItems = Align.Center;
                this.style.justifyContent = Justify.Center;
                this.style.marginTop = 0;
                this.style.marginBottom = 0;
                this.style.marginLeft = 0;
                this.style.marginRight = 0;
                this.style.paddingTop = 1;
                this.style.paddingBottom = 1;
                this.style.paddingLeft = 1;
                this.style.paddingRight = 1;
                // 图标
                this.Add(iconImage);
                this.SetIcon(icon);
                // 文本
                this.Add(textLabel);
                this.SetText(text);
            }

            public void SetIcon(Texture icon)
            {
                this.iconImage.image = icon;
                this.iconImage.style.display = (icon == null ? DisplayStyle.None : DisplayStyle.Flex);
            }

            public void SetIconSize(int size)
            {
                this.iconImage.style.width = size;
            }

            public void SetText(string text)
            {
                this.textLabel.text = text;
                this.textLabel.style.display = (string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex);
            }

            public void SetTextFontSize(int size)
            {
                this.textLabel.style.fontSize = size;
            }

            public void SetTextFontStyle(StyleEnum<FontStyle> fontStyle)
            {
                this.textLabel.style.unityFontStyleAndWeight = fontStyle;
            }

        }

    }

}
