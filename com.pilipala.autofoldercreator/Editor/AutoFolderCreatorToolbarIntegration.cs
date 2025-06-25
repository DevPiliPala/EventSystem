using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Pilipala.AutoFolderCreator.Editor
{
    /// <summary>
    /// Provides keyboard shortcuts and additional menu integration for Auto Folder Creator
    /// </summary>
    [InitializeOnLoad]
    public static class AutoFolderCreatorToolbarIntegration
    {
        static AutoFolderCreatorToolbarIntegration()
        {
            // Register for the toolbar callback
            EditorApplication.update += OnUpdate;
        }
        
        private static void OnUpdate()
        {
            // This is a simpler approach that doesn't try to draw GUI elements
            // Instead, we'll just ensure our menu items are available
            // The toolbar integration will be handled through menu items only
        }
        
        /// <summary>
        /// Quick create folder structure with keyboard shortcut (Ctrl+Shift+F)
        /// </summary>
        [MenuItem("PiliPala/Tools/Auto Folder Creator/Quick Create %#f", false, 1)]
        public static void QuickCreateWithShortcut()
        {
            AutoFolderCreatorWindow.CreateFolderStructure();
        }
        
        /// <summary>
        /// Validate the quick create menu item
        /// </summary>
        [MenuItem("PiliPala/Tools/Auto Folder Creator/Quick Create %#f", true)]
        public static bool ValidateQuickCreate()
        {
            // Always allow the action
            return true;
        }
    }
} 