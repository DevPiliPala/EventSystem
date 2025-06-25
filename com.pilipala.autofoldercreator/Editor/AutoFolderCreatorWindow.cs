using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Pilipala.AutoFolderCreator.Editor
{
    /// <summary>
    /// Editor window for automatically creating a comprehensive folder structure
    /// </summary>
    public class AutoFolderCreatorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private bool _showCodeFolders = true;
        private bool _showArtFolders = true;
        private bool _showAudioFolders = true;
        private bool _showPrefabFolders = true;
        private bool _showSceneFolders = true;
        private bool _showResourceFolders = true;
        private bool _showPluginFolders = true;
        
        private string _projectName = "MyGame";
        private bool _useProjectName = false;
        
        [MenuItem("PiliPala/Tools/Auto Folder Creator")]
        public static void ShowWindow()
        {
            GetWindow<AutoFolderCreatorWindow>("Auto Folder Creator");
        }
        
        [MenuItem("PiliPala/Tools/Auto Folder Creator/Create Folder Structure")]
        public static void CreateFolderStructure()
        {
            var window = GetWindow<AutoFolderCreatorWindow>("Auto Folder Creator");
            window.CreateAllFolders();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Auto Folder Creator", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Create a comprehensive folder structure for your Unity project", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            
            // Project name section
            EditorGUILayout.BeginHorizontal();
            _useProjectName = EditorGUILayout.Toggle("Use Project Name", _useProjectName);
            if (_useProjectName)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Project Name", Application.productName);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                _projectName = EditorGUILayout.TextField("Custom Name", _projectName);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Folder structure options
            EditorGUILayout.LabelField("Folder Structure", EditorStyles.boldLabel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            // Code folders
            _showCodeFolders = EditorGUILayout.Foldout(_showCodeFolders, "Code Structure", true);
            if (_showCodeFolders)
            {
                EditorGUI.indentLevel++;
                DrawCodeFolders();
                EditorGUI.indentLevel--;
            }
            
            // Art folders
            _showArtFolders = EditorGUILayout.Foldout(_showArtFolders, "Art Structure", true);
            if (_showArtFolders)
            {
                EditorGUI.indentLevel++;
                DrawArtFolders();
                EditorGUI.indentLevel--;
            }
            
            // Audio folders
            _showAudioFolders = EditorGUILayout.Foldout(_showAudioFolders, "Audio Structure", true);
            if (_showAudioFolders)
            {
                EditorGUI.indentLevel++;
                DrawAudioFolders();
                EditorGUI.indentLevel--;
            }
            
            // Prefab folders
            _showPrefabFolders = EditorGUILayout.Foldout(_showPrefabFolders, "Prefab Structure", true);
            if (_showPrefabFolders)
            {
                EditorGUI.indentLevel++;
                DrawPrefabFolders();
                EditorGUI.indentLevel--;
            }
            
            // Scene folders
            _showSceneFolders = EditorGUILayout.Foldout(_showSceneFolders, "Scene Structure", true);
            if (_showSceneFolders)
            {
                EditorGUI.indentLevel++;
                DrawSceneFolders();
                EditorGUI.indentLevel--;
            }
            
            // Resource folders
            _showResourceFolders = EditorGUILayout.Foldout(_showResourceFolders, "Resource Structure", true);
            if (_showResourceFolders)
            {
                EditorGUI.indentLevel++;
                DrawResourceFolders();
                EditorGUI.indentLevel--;
            }
            
            // Plugin folders
            _showPluginFolders = EditorGUILayout.Foldout(_showPluginFolders, "Plugin Structure", true);
            if (_showPluginFolders)
            {
                EditorGUI.indentLevel++;
                DrawPluginFolders();
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            // Action buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Create All Folders", GUILayout.Height(30)))
            {
                CreateAllFolders();
            }
            
            if (GUILayout.Button("Create Selected", GUILayout.Height(30)))
            {
                CreateSelectedFolders();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(25)))
            {
                ResetToDefaults();
            }
            
            if (GUILayout.Button("Export Structure", GUILayout.Height(25)))
            {
                ExportFolderStructure();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawCodeFolders()
        {
            EditorGUILayout.LabelField("Core/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── Events/");
            EditorGUILayout.LabelField("├── Utilities/");
            EditorGUILayout.LabelField("├── Config/");
            EditorGUILayout.LabelField("└── [Other Core Systems]/");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("Systems/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── SaveSystem/");
            EditorGUILayout.LabelField("├── Audio/");
            EditorGUILayout.LabelField("├── Data/");
            EditorGUILayout.LabelField("├── Networking/");
            EditorGUILayout.LabelField("└── [Other Systems]/");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("Gameplay/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── Characters/");
            EditorGUILayout.LabelField("├── World/");
            EditorGUILayout.LabelField("├── Items/");
            EditorGUILayout.LabelField("├── Mechanics/");
            EditorGUILayout.LabelField("└── [Other Gameplay]/");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("UI/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── Menus/");
            EditorGUILayout.LabelField("├── HUD/");
            EditorGUILayout.LabelField("├── Common/");
            EditorGUILayout.LabelField("└── [Other UI]/");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("Input/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── Controllers/");
            EditorGUILayout.LabelField("├── Bindings/");
            EditorGUILayout.LabelField("└── [Other Input]/");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("Editor/");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("├── Tools/");
            EditorGUILayout.LabelField("├── Inspectors/");
            EditorGUILayout.LabelField("└── [Other Editor Scripts]/");
            EditorGUI.indentLevel--;
        }
        
        private void DrawArtFolders()
        {
            EditorGUILayout.LabelField("2D/");
            EditorGUILayout.LabelField("3D/");
            EditorGUILayout.LabelField("Materials/");
            EditorGUILayout.LabelField("Animations/");
            EditorGUILayout.LabelField("[Other Art Assets]/");
        }
        
        private void DrawAudioFolders()
        {
            EditorGUILayout.LabelField("Music/");
            EditorGUILayout.LabelField("SFX/");
            EditorGUILayout.LabelField("[Other Audio Assets]/");
        }
        
        private void DrawPrefabFolders()
        {
            EditorGUILayout.LabelField("Characters/");
            EditorGUILayout.LabelField("World/");
            EditorGUILayout.LabelField("UI/");
            EditorGUILayout.LabelField("Items/");
            EditorGUILayout.LabelField("[Other Prefabs]/");
        }
        
        private void DrawSceneFolders()
        {
            EditorGUILayout.LabelField("Menus/");
            EditorGUILayout.LabelField("Levels/");
            EditorGUILayout.LabelField("Test/");
            EditorGUILayout.LabelField("[Other Scenes]/");
        }
        
        private void DrawResourceFolders()
        {
            EditorGUILayout.LabelField("[Resource Files]/");
        }
        
        private void DrawPluginFolders()
        {
            EditorGUILayout.LabelField("[Third-Party Plugins]/");
        }
        
        private void CreateAllFolders()
        {
            var folderStructure = GetCompleteFolderStructure();
            CreateFolders(folderStructure);
            
            // Create FOLDERSTRUCTURE.MD file
            CreateFolderStructureDocumentation();
            
            EditorUtility.DisplayDialog("Success", "Folder structure created successfully!", "OK");
            AssetDatabase.Refresh();
        }
        
        private void CreateSelectedFolders()
        {
            // For now, just create all folders
            // Could be enhanced to only create selected ones
            CreateAllFolders();
        }
        
        private void ResetToDefaults()
        {
            _showCodeFolders = true;
            _showArtFolders = true;
            _showAudioFolders = true;
            _showPrefabFolders = true;
            _showSceneFolders = true;
            _showResourceFolders = true;
            _showPluginFolders = true;
            
            _projectName = "MyGame";
            _useProjectName = false;
        }
        
        private void ExportFolderStructure()
        {
            var folderStructure = GetCompleteFolderStructure();
            var structureText = ConvertToText(folderStructure);
            
            var path = EditorUtility.SaveFilePanel("Export Folder Structure", "", "FolderStructure", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, structureText);
                EditorUtility.DisplayDialog("Success", "Folder structure exported successfully!", "OK");
            }
        }
        
        private List<string> GetCompleteFolderStructure()
        {
            var folders = new List<string>();
            
            // Code structure
            folders.AddRange(new string[]
            {
                "Assets/Code",
                "Assets/Code/Core",
                "Assets/Code/Core/Events",
                "Assets/Code/Core/Utilities",
                "Assets/Code/Core/Config",
                "Assets/Code/Systems",
                "Assets/Code/Systems/SaveSystem",
                "Assets/Code/Systems/Audio",
                "Assets/Code/Systems/Data",
                "Assets/Code/Systems/Networking",
                "Assets/Code/Gameplay",
                "Assets/Code/Gameplay/Characters",
                "Assets/Code/Gameplay/World",
                "Assets/Code/Gameplay/Items",
                "Assets/Code/Gameplay/Mechanics",
                "Assets/Code/UI",
                "Assets/Code/UI/Menus",
                "Assets/Code/UI/HUD",
                "Assets/Code/UI/Common",
                "Assets/Code/Input",
                "Assets/Code/Input/Controllers",
                "Assets/Code/Input/Bindings",
                "Assets/Code/Editor",
                "Assets/Code/Editor/Tools",
                "Assets/Code/Editor/Inspectors"
            });
            
            // Art structure
            folders.AddRange(new string[]
            {
                "Assets/Art",
                "Assets/Art/2D",
                "Assets/Art/3D",
                "Assets/Art/Materials",
                "Assets/Art/Animations"
            });
            
            // Audio structure
            folders.AddRange(new string[]
            {
                "Assets/Audio",
                "Assets/Audio/Music",
                "Assets/Audio/SFX"
            });
            
            // Prefab structure
            folders.AddRange(new string[]
            {
                "Assets/Prefabs",
                "Assets/Prefabs/Characters",
                "Assets/Prefabs/World",
                "Assets/Prefabs/UI",
                "Assets/Prefabs/Items"
            });
            
            // Scene structure
            folders.AddRange(new string[]
            {
                "Assets/Scenes",
                "Assets/Scenes/Menus",
                "Assets/Scenes/Levels",
                "Assets/Scenes/Test"
            });
            
            // Resource structure
            folders.AddRange(new string[]
            {
                "Assets/Resources"
            });
            
            // Plugin structure
            folders.AddRange(new string[]
            {
                "Assets/Plugins"
            });
            
            return folders;
        }
        
        private void CreateFolders(List<string> folders)
        {
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
        
        private void CreateFolderStructureDocumentation()
        {
            var projectName = _useProjectName ? Application.productName : _projectName;
            var readmePath = "Assets/FOLDERSTRUCTURE.md";
            
            var content = GenerateFolderStructureDocumentation(projectName);
            File.WriteAllText(readmePath, content);
        }
        
        private string GenerateFolderStructureDocumentation(string projectName)
        {
            return $@"# {projectName} - Folder Structure

This document explains the folder structure and organization of the {projectName} Unity project.

## Overview

This project follows a comprehensive folder structure designed for scalability, maintainability, and team collaboration. The structure is flexible enough to support any type of game (2D, 3D, any genre) while maintaining clear organization and separation of concerns.

## Root Structure

```
Assets/
├── Code/           # All C# scripts and code files
├── Art/            # Visual assets (2D, 3D, materials, animations)
├── Audio/          # Audio assets (music, sound effects)
├── Prefabs/        # Unity prefabs
├── Scenes/         # Unity scenes
├── Resources/      # Runtime-loaded resources
└── Plugins/        # Third-party plugins
```

## Code Structure

### Core/
Contains fundamental systems and utilities that are used throughout the project.

- **Events/**: Event system and messaging infrastructure
- **Utilities/**: Helper classes and utility functions
- **Config/**: Configuration files and settings
- **[Other Core Systems]/**: Additional core systems

### Systems/
Contains systems that provide foundational services to the game.

- **SaveSystem/**: Save/load functionality
- **Audio/**: Audio management and control systems
- **Data/**: Data structures and persistence
- **Networking/**: Multiplayer and network functionality
- **[Other Systems]/**: Additional system services

### Gameplay/
Contains all gameplay-related scripts and systems.

- **Characters/**: Character controllers, AI, and behavior scripts
- **World/**: World generation, environment, and level management
- **Items/**: Item system, inventory, and collectibles
- **Mechanics/**: Core gameplay mechanics and systems
- **[Other Gameplay]/**: Additional gameplay systems

### UI/
Contains UI and presentation layer scripts.

- **Menus/**: Menu systems and navigation
- **HUD/**: Heads-up display elements
- **Common/**: Shared UI components and utilities
- **[Other UI]/**: Additional UI elements

### Input/
Contains input handling and controller scripts.

- **Controllers/**: Input controller implementations
- **Bindings/**: Input binding configurations
- **[Other Input]/**: Additional input systems

### Editor/
Contains custom editor tools and inspectors.

- **Tools/**: Custom editor tools
- **Inspectors/**: Custom inspector implementations
- **[Other Editor Scripts]/**: Additional editor functionality

## Asset Structure

### Art/
Contains all visual assets for the project, organized by type.

- **2D/**: 2D sprites, textures, and UI graphics
- **3D/**: 3D models, textures, and assets
- **Materials/**: Material assets for both 2D and 3D
- **Animations/**: Animation clips and controllers
- **[Other Art Assets]/**: Additional visual assets

### Audio/
Contains all audio assets for the project.

- **Music/**: Background music and soundtrack
- **SFX/**: Sound effects and audio clips
- **[Other Audio Assets]/**: Additional audio assets

### Prefabs/
Contains Unity prefabs organized by type.

- **Characters/**: Character prefabs (player, NPCs, enemies)
- **World/**: World objects, environment, and level elements
- **UI/**: User interface prefabs
- **Items/**: Item and object prefabs
- **[Other Prefabs]/**: Additional prefab types

### Scenes/
Contains Unity scenes organized by purpose.

- **Menus/**: Menu and navigation scenes
- **Levels/**: Gameplay levels and areas
- **Test/**: Testing and development scenes
- **[Other Scenes]/**: Additional scene types

### Resources/
Contains assets that need to be loaded at runtime using Resources.Load().

- **[Resource Files]/**: Runtime-loaded assets

### Plugins/
Contains third-party plugins and external assets.

- **[Third-Party Plugins]/**: External plugins and assets

## Guidelines

### File Naming
- Use PascalCase for script files (e.g., PlayerController.cs)
- Use descriptive names that clearly indicate the file's purpose
- Avoid abbreviations unless they are widely understood

### Organization
- Keep related files together in appropriate folders
- Don't create too many subfolders - aim for logical grouping
- Use the [Other] folders for assets that don't fit the main categories

### Scripts
- Place scripts in the appropriate Code subfolder based on their purpose
- Core systems go in Core/
- Gameplay mechanics go in Gameplay/
- UI elements go in UI/
- Editor tools go in Editor/

### Assets
- Organize assets by type and purpose
- Use consistent naming conventions
- Keep the folder structure clean and organized

## Best Practices

1. **Consistency**: Follow the established folder structure consistently
2. **Clarity**: Use clear, descriptive names for files and folders
3. **Organization**: Keep related assets together
4. **Scalability**: Structure should support project growth
5. **Team Collaboration**: Structure should be intuitive for team members
6. **Flexibility**: Structure should work for any game type or genre

## Maintenance

- Regularly review and update this structure as the project evolves
- Remove empty folders that are no longer needed
- Update this documentation when adding new folder types
- Ensure all team members understand and follow the structure

## Game Type Adaptations

### 2D Games
- Use Art/2D/ for sprites and textures
- Focus on Code/Gameplay/Characters for 2D character controllers
- Use Code/Gameplay/World for 2D level management

### 3D Games
- Use Art/3D/ for models and 3D assets
- Focus on Code/Gameplay/Characters for 3D character controllers
- Use Code/Gameplay/World for 3D world management

### Any Genre
- Adapt the Gameplay/Mechanics folder for genre-specific systems
- Use the flexible structure to add genre-specific folders as needed
- Maintain the core organization principles

This folder structure is designed to support efficient development, easy navigation, and successful team collaboration for any type of Unity game project.
";
        }
        
        private string ConvertToText(List<string> folders)
        {
            var text = "Folder Structure:\n\n";
            foreach (var folder in folders)
            {
                text += folder + "\n";
            }
            return text;
        }
    }
} 