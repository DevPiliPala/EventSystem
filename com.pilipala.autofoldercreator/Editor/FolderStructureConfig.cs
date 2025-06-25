using UnityEngine;
using System;
using System.Collections.Generic;

namespace Pilipala.AutoFolderCreator.Editor
{
    /// <summary>
    /// Configuration for folder structure templates
    /// </summary>
    [CreateAssetMenu(fileName = "FolderStructureConfig", menuName = "ScriptableObjects/AutoFolderCreator/FolderStructureConfig", order = 1)]
    public class FolderStructureConfig : ScriptableObject
    {
        [Header("Template Configuration")]
        [SerializeField] private string templateName = "Default";
        [SerializeField] private string description = "Default folder structure for Unity projects";
        
        [Header("Folder Structure")]
        [SerializeField] private List<FolderCategory> categories = new List<FolderCategory>();
        
        [Header("Options")]
        [SerializeField] private bool createReadmeFiles = true;
        [SerializeField] private bool createGitkeepFiles = true;
        [SerializeField] private bool createMetaFiles = true;
        
        public string TemplateName => templateName;
        public string Description => description;
        public List<FolderCategory> Categories => categories;
        public bool CreateReadmeFiles => createReadmeFiles;
        public bool CreateGitkeepFiles => createGitkeepFiles;
        public bool CreateMetaFiles => createMetaFiles;
        
        /// <summary>
        /// Get all folders from this configuration
        /// </summary>
        public List<string> GetAllFolders()
        {
            var folders = new List<string>();
            
            foreach (var category in categories)
            {
                if (category.IsEnabled)
                {
                    folders.AddRange(category.GetAllFolders());
                }
            }
            
            return folders;
        }
    }
    
    /// <summary>
    /// Represents a category of folders (e.g., Code, Art, Audio)
    /// </summary>
    [Serializable]
    public class FolderCategory
    {
        [SerializeField] private string categoryName = "New Category";
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private List<FolderItem> folders = new List<FolderItem>();
        
        public string CategoryName => categoryName;
        public bool IsEnabled => isEnabled;
        public List<FolderItem> Folders => folders;
        
        public List<string> GetAllFolders()
        {
            var folders = new List<string>();
            
            foreach (var folder in this.folders)
            {
                if (folder.IsEnabled)
                {
                    folders.Add(folder.FolderPath);
                    
                    if (folder.Subfolders != null)
                    {
                        foreach (var subfolder in folder.Subfolders)
                        {
                            if (subfolder.IsEnabled)
                            {
                                folders.Add(subfolder.FolderPath);
                            }
                        }
                    }
                }
            }
            
            return folders;
        }
    }
    
    /// <summary>
    /// Represents a single folder item
    /// </summary>
    [Serializable]
    public class FolderItem
    {
        [SerializeField] private string name = "New Folder";
        [SerializeField] private string folderPath = "Assets/NewFolder";
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private List<FolderItem> subfolders = new List<FolderItem>();
        
        public string Name => name;
        public string FolderPath => folderPath;
        public bool IsEnabled => isEnabled;
        public List<FolderItem> Subfolders => subfolders;
    }
} 