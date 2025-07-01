# Auto Folder Creator

Creates a proper folder structure for your Unity project so you don't have to. Perfect for game jams when you want to start coding, not organizing folders.

## What's this for?

Quick setup of file structures for game jams.

Hit **Ctrl+Shift+F** and boom - you've got folders for everything:
- Scripts organized by purpose (Core, Gameplay, UI, etc.)
- Art assets sorted by type
- Scenes in their own spots
- A README that explains what goes where

Takes 2 seconds and you never have to think about project organization again.

## How to use it

**The fast way:** Press **Ctrl+Shift+F** (or **Cmd+Shift+F** on Mac). Done.

**The menu way:** Tools → Auto Folder Creator → Quick Create

**The fancy way:** Window → Auto Folder Creator (if you want to customize stuff first)

## What folders you get

```
Assets/
├── Code/
│   ├── Core/               // EventBus, utilities, config
│   ├── Infrastructure/     // Save system, audio, analytics
│   ├── Gameplay/          // Player, enemies, levels, items
│   ├── Presentation/      // UI, menus, animations
│   ├── Input/             // Input handling
│   └── Editor/            // Editor scripts
│
├── Art/
│   ├── Sprites/
│   ├── Textures/
│   ├── Animations/
│   └── Materials/
│
├── Audio/
│   ├── Music/
│   └── SFX/
│
├── Prefabs/
│   ├── Characters/
│   ├── Enemies/
│   ├── UI/
│   └── Items/
│
├── Scenes/
│   ├── MainMenu/
│   ├── Levels/
│   └── Test/
│
├── Resources/
├── StreamingAssets/
├── Plugins/
└── FOLDERSTRUCTURE.md     // Explains what goes where
```

Plus it creates a FOLDERSTRUCTURE.md file that explains what each folder is for, so your teammates (or future you) know where stuff goes.

## Why this is useful

**For game jams:** Hit the shortcut, start coding immediately. No time wasted on "where should this go?"

**For team projects:** Everyone knows where to put stuff. No more hunting through random folders.

**For your sanity:** Your project stays organized even when you're rushing to meet deadlines.

## Customization

If you want to tweak the folder structure:
1. Open Window → Auto Folder Creator
2. Modify what you want
3. Hit "Create All Folders"

You can also export/import folder structures to share between projects.

## Works great with other Pilipala packages

- **Event System** goes in Code/Core/EventBus/
- **Game State System** goes in Code/Infrastructure/
- **Movement System** goes in Code/Gameplay/Player/

## If something breaks

**Folders not showing up:** Check the console for errors, make sure Unity has write permissions.

**Shortcut not working:** Restart Unity, or use the menu instead.

**Already have folders:** It won't overwrite existing stuff, so you're safe to run it multiple times.

That's it. Simple tool, saves time, keeps you organized. Perfect for when you just want to start making a game. 