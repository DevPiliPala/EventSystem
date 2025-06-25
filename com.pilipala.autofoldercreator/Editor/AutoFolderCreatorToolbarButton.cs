using UnityEngine;
using UnityEditor;

namespace Pilipala.AutoFolderCreator.Editor
{
    /// <summary>
    /// Provides menu items for the Auto Folder Creator tool
    /// </summary>
    public static class AutoFolderCreatorToolbarButton
    {
        [MenuItem("PiliPala/Tools/Auto Folder Creator/Quick Create", false, 1)]
        public static void QuickCreate()
        {
            AutoFolderCreatorWindow.CreateFolderStructure();
        }
        
        [MenuItem("PiliPala/Tools/Auto Folder Creator/Open Window", false, 2)]
        public static void OpenWindow()
        {
            AutoFolderCreatorWindow.ShowWindow();
        }
        
        [MenuItem("PiliPala/Tools/Auto Folder Creator/About", false, 100)]
        public static void About()
        {
            EditorUtility.DisplayDialog("Auto Folder Creator", 
                "Auto Folder Creator v1.0\n\n" +
                "A tool for automatically creating a comprehensive folder structure for Unity projects.\n\n" +
                "Created by PiliPala", "OK");
        }
        
        // Separator
        [MenuItem("PiliPala/Tools/Auto Folder Creator/", false, 50)]
        public static void Separator()
        {
            // This creates a separator in the menu
        }
    }
} 