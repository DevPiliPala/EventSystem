# Auto Folder Creator

A Unity editor tool that automatically creates a comprehensive folder structure for game development projects, streamlining the setup process for game jams and new projects.

## Features

- **One-Click Setup**: Create a complete folder structure with a single click
- **Comprehensive Organization**: Pre-configured folders for all aspects of game development
- **Single Documentation**: Creates a detailed FOLDERSTRUCTURE.MD file in the Assets root
- **Custom Project Names**: Use Unity's project name or set a custom name
- **Menu Integration**: Quick access from Unity's Tools menu
- **Keyboard Shortcut**: Use Ctrl+Shift+F (Cmd+Shift+F on Mac) for instant creation
- **Export/Import**: Share folder structures between projects

## Quick Start

### Method 1: Keyboard Shortcut (Fastest)
- Press **Ctrl+Shift+F** (Windows) or **Cmd+Shift+F** (Mac)
- Folder structure is created instantly

### Method 2: Menu Access
1. Go to `Tools > Auto Folder Creator > Quick Create`
2. Or open the full window: `Tools > Auto Folder Creator > Open Window`

### Method 3: Window
1. Go to `Window > Auto Folder Creator`
2. Configure project name and click "Create All Folders"

## Folder Structure

The tool creates the following comprehensive structure:

```
Assets/
│
├── Code/
│   ├── Core/
│   │   ├── EventBus/
│   │   ├── Utilities/
│   │   ├── Config/
│   │   └── [Other Core Systems]/
│   │
│   ├── Infrastructure/
│   │   ├── Achievements/
│   │   ├── SaveSystem/
│   │   ├── Audio/
│   │   ├── Data/
│   │   ├── Networking/
│   │   ├── Analytics/
│   │   └── [Other Infrastructure]/
│   │
│   ├── Gameplay/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Levels/
│   │   ├── Items/
│   │   ├── Combat/
│   │   ├── Progression/
│   │   └── [Other Gameplay Systems]/
│   │
│   ├── Presentation/
│   │   ├── MainMenu/
│   │   │   ├── MenuButtons/
│   │   │   └── [Other MainMenu UI]/
│   │   ├── Achievements/
│   │   ├── HUD/
│   │   ├── UICommon/
│   │   ├── Animations/
│   │   ├── Audio/
│   │   └── [Other Presentation]/
│   │
│   ├── Input/
│   │   ├── Controllers/
│   │   ├── Bindings/
│   │   └── [Other Input]/
│   │
│   └── Editor/
│       ├── Tools/
│       ├── Inspectors/
│       └── [Other Editor Scripts]/
│
├── Art/
│   ├── Sprites/
│   ├── Textures/
│   ├── Animations/
│   ├── Materials/
│   └── [Other Art Assets]/
│
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── [Other Audio Assets]/
│
├── Prefabs/
│   ├── Characters/
│   ├── Enemies/
│   ├── UI/
│   ├── Items/
│   └── [Other Prefabs]/
│
├── Scenes/
│   ├── MainMenu/
│   ├── Levels/
│   ├── Test/
│   └── [Other Scenes]/
│
├── Resources/
│   └── [Resource Files]/
│
├── StreamingAssets/
│   └── [Streaming Files]/
│
├── Plugins/
│   └── [Third-Party Plugins]/
│
└── FOLDERSTRUCTURE.md
```

## Configuration Options

### Project Name
- **Use Project Name**: Automatically use Unity's project name
- **Custom Name**: Set a custom project name in the text field

### Folder Categories
- **Code Structure**: Core, Infrastructure, Gameplay, Presentation, Input, Editor
- **Art Structure**: Sprites, Textures, Animations, Materials
- **Audio Structure**: Music, SFX
- **Prefab Structure**: Characters, Enemies, UI, Items
- **Scene Structure**: MainMenu, Levels, Test
- **Resource Structure**: Resources folder
- **Plugin Structure**: Plugins folder

## Documentation

The tool automatically creates a comprehensive `FOLDERSTRUCTURE.md` file in the Assets root that includes:

- **Project Overview**: Explanation of the folder structure
- **Detailed Descriptions**: What each folder contains and its purpose
- **Guidelines**: File naming and organization best practices
- **Best Practices**: Tips for maintaining the structure
- **Maintenance**: How to keep the structure organized

## Game Jam Workflow

### Setup Phase (10 seconds)
1. Press **Ctrl+Shift+F** (or **Cmd+Shift+F** on Mac)
2. Start developing immediately

### Development Phase
- **Organized Assets**: All assets go in their proper folders
- **Quick Access**: Easy to find files during development
- **Team Collaboration**: Consistent structure for team members
- **Clear Documentation**: FOLDERSTRUCTURE.md explains everything

### Polish Phase
- **Professional Structure**: Clean, organized project structure
- **Easy Maintenance**: Clear separation of concerns
- **Team Handoff**: Comprehensive documentation for project handoff

## Customization

### Creating Custom Templates
1. Open Auto Folder Creator window
2. Modify folder structure as needed
3. Export structure to file
4. Share with team or reuse in other projects

### Modifying Default Structure
1. Edit the `GetCompleteFolderStructure()` method
2. Add or remove folders as needed
3. Rebuild the package

### Adding New Categories
1. Add new category to the folder structure
2. Update the drawing methods
3. Add to the creation logic

## Integration

### With Other Pilipala Packages
- **Event System**: Code/EventBus folder ready for event system
- **Movement System**: Code/Gameplay/Player folder for movement scripts
- **Game State System**: Code/Infrastructure folder for state management

### With Unity Features
- **Build Settings**: Scenes folder organized for build settings
- **Asset Bundles**: Resources folder for asset bundle management
- **Editor Scripts**: Editor folder for custom tools

## Troubleshooting

### Common Issues

**Folders not created:**
- Check if folders already exist
- Verify write permissions
- Check Unity console for errors

**Keyboard shortcut not working:**
- Restart Unity
- Check if package is properly imported
- Verify assembly definition is correct

**FOLDERSTRUCTURE.md not created:**
- Check if Assets folder is writable
- Verify the file wasn't created with a different name
- Check Unity console for error messages

### Debug Tips
- Use the window version for detailed feedback
- Check Unity console for error messages
- Verify folder paths are valid
- Test with a new project first

## Best Practices

### For Game Jams
1. **Use Keyboard Shortcut**: Ctrl+Shift+F for instant setup
2. **Customize Later**: Modify structure as project grows
3. **Keep It Simple**: Don't over-organize for small projects
4. **Read Documentation**: Check FOLDERSTRUCTURE.md for guidance

### For Larger Projects
1. **Plan Structure**: Design folder structure before starting
2. **Use Templates**: Create project-specific templates
3. **Team Standards**: Establish consistent naming conventions
4. **Regular Maintenance**: Keep structure organized as project grows

### For Teams
1. **Share Templates**: Export and share folder structures
2. **Document Standards**: Create team guidelines
3. **Review Documentation**: Ensure FOLDERSTRUCTURE.md is up to date
4. **Regular Reviews**: Periodically review and update structure

## API Reference

### Menu Items
- `Tools > Auto Folder Creator > Quick Create`: Instant folder creation
- `Tools > Auto Folder Creator > Open Window`: Open configuration window
- `Window > Auto Folder Creator`: Open main window

### Keyboard Shortcuts
- **Ctrl+Shift+F** (Windows) / **Cmd+Shift+F** (Mac): Quick Create

### Static Methods
```csharp
// Create default folder structure
AutoFolderCreatorWindow.CreateFolderStructure();

// Open configuration window
AutoFolderCreatorWindow.ShowWindow();
```

### Configuration
```csharp
// Access folder structure configuration
var config = ScriptableObject.CreateInstance<FolderStructureConfig>();

// Get all folders from configuration
var folders = config.GetAllFolders();
```

## Contributing

### Adding New Features
1. Fork the repository
2. Create feature branch
3. Implement changes
4. Test thoroughly
5. Submit pull request

### Reporting Issues
1. Check existing issues
2. Create detailed bug report
3. Include Unity version
4. Provide reproduction steps

## License

This package is licensed under the MIT License. See LICENSE file for details.

## Support

For support and questions:
- Check the documentation
- Search existing issues
- Create new issue with details
- Contact the maintainer

This tool is designed to make Unity project setup faster and more organized, especially for game jams where time is precious and organization is key to success. 