using UnityEditor;
using UnityEngine;

namespace ChenPipi.ProjectPinBoard.Editor
{

    /// <summary>
    /// 窗口
    /// </summary>
    public partial class ProjectPinBoardWindow : EditorWindow, IHasCustomMenu
    {

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, Menu_Reload);
            menu.AddItem(new GUIContent("Show Serialized Data File"), false, Menu_ShowSerializedDataFile);
            menu.AddItem(new GUIContent("Show Serialized Settings File"), false, Menu_ShowSerializedSettingsFile);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Clear Data ⚠️"), false, Menu_ClearData);
            menu.AddItem(new GUIContent("Reset Settings ⚠️"), false, Menu_ResetSettings);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("About/Home Page (Gitee)"), false, Menu_HomePage1);
            menu.AddItem(new GUIContent("About/Home Page (Github)"), false, Menu_HomePage2);
        }

        private void Menu_Reload()
        {
            ProjectPinBoardManager.ReloadData();
            ProjectPinBoardManager.ReloadSettings();
            ApplySettings();
        }

        private void Menu_ShowSerializedDataFile()
        {
            EditorUtility.RevealInFinder(ProjectPinBoardData.SerializedFilePath);
        }

        private void Menu_ShowSerializedSettingsFile()
        {
            EditorUtility.RevealInFinder(ProjectPinBoardSettings.SerializedFilePath);
        }

        private void Menu_ClearData()
        {
            bool isOk = EditorUtility.DisplayDialog(
                "[Project Pin Board] Clear Data",
                "Are you sure to clear the data? This operation cannot be undone!",
                "Confirm!",
                "Cancel"
            );
            if (isOk) ProjectPinBoardManager.ClearData();
        }

        private void Menu_ResetSettings()
        {
            ProjectPinBoardManager.ResetSettings();
            ApplySettings();
        }

        private void Menu_HomePage1()
        {
            Application.OpenURL("https://gitee.com/ichenpipi/unity-project-pin-board");
        }

        private void Menu_HomePage2()
        {
            Application.OpenURL("https://github.com/ichenpipi/unity-project-pin-board");
        }

    }

}
