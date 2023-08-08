using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow
    {

        private static Color separatorColor => Theme.GetColor("SeparatorColor");

        private static Color dragLineColor => Theme.GetColor("DragLineColor");

        private static Color popupBgColor => Theme.GetColor("PopupBgColor");

        private static Color textColor => Theme.GetColor("TextColor");
        private static Color borderColor => Theme.GetColor("BorderColor");

        private static Color tagBgColor => Theme.GetColor("TagBgColor");
        private static Color tagTextColor => Theme.GetColor("TagTextColor");

        private static Color dropTipBgColor => Theme.GetColor("DropTipBgColor");
        private static Color dropTipBorderColor => Theme.GetColor("DropTipBorderColor");
        private static Color dropTipTextColor => Theme.GetColor("DropTipTextColor");

        private static class Theme
        {

            public static Color GetColor(string name)
            {
                if (isDarkTheme)
                {
                    if (s_DarkColor.TryGetValue(name, out Color color)) return color;
                }
                else
                {
                    if (s_LightColor.TryGetValue(name, out Color color)) return color;
                }
                return s_DefaultColor;
            }

            public static bool isDarkTheme => EditorGUIUtility.isProSkin;

            private static readonly Color s_DefaultColor = Color.black;

            private static readonly Dictionary<string, Color> s_DarkColor = new Dictionary<string, Color>()
            {
                { "SeparatorColor", new Color(35 / 255f, 35 / 255f, 35 / 255f, 1) },

                { "DragLineColor", new Color(89 / 255f, 89 / 255f, 89 / 255f, 1) },

                { "TextColor", new Color(196 / 255f, 196 / 255f, 196 / 255f, 1) },
                { "BorderColor", new Color(102 / 255f, 102 / 255f, 102 / 255f, 1) },

                { "PopupBgColor", new Color(57 / 255f, 57 / 255f, 57 / 255f, 1) },

                { "TagBgColor", new Color(34 / 255f, 73 / 255f, 128 / 255f, 1) },
                { "TagTextColor", new Color(196 / 255f, 196 / 255f, 196 / 255f, 1) },

                { "DropTipBgColor", new Color(0f, 0f, 0f, 80 / 255f) },
                { "DropTipBorderColor", new Color(210 / 255f, 210 / 255f, 210 / 255f, 1) },
                { "DropTipTextColor", new Color(210 / 255f, 210 / 255f, 210 / 255f, 1) },

                { "ListItemNormalColor", new Color(0, 0, 0, 0) },
                { "ListItemActiveColor", new Color(44 / 255f, 93 / 255f, 135 / 255f, 1) },
            };

            private static readonly Dictionary<string, Color> s_LightColor = new Dictionary<string, Color>()
            {
                { "SeparatorColor", new Color(153 / 255f, 153 / 255f, 153 / 255f, 1) },

                { "DragLineColor", new Color(153 / 255f, 153 / 255f, 153 / 255f, 1) },

                { "TextColor", new Color(250 / 255f, 250 / 255f, 250 / 255f, 1) },
                { "BorderColor", new Color(153 / 255f, 153 / 255f, 153 / 255f, 1) },

                { "PopupBgColor", new Color(222 / 255f, 222 / 255f, 222 / 255f, 1) },

                { "TagBgColor", new Color(20 / 255f, 100 / 255f, 200 / 255f, 1) },
                { "TagTextColor", new Color(250 / 255f, 250 / 255f, 250 / 255f, 1) },

                { "DropTipBgColor", new Color(0f, 0f, 0f, 160 / 255f) },
                { "DropTipBorderColor", new Color(210 / 255f, 210 / 255f, 210 / 255f, 1) },
                { "DropTipTextColor", new Color(210 / 255f, 210 / 255f, 210 / 255f, 1) },

                { "ListItemNormalColor", new Color(0, 0, 0, 0) },
                { "ListItemActiveColor", new Color(46 / 255f, 91 / 255f, 141 / 255f, 1) },
            };

        }

    }

}
