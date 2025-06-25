# Game State System

A flexible game state management system for Unity with scene management, pause functionality, and save system integration. Designed for rapid game jam development with event-driven architecture.

## Features

- **State Management**: Comprehensive state system with configurable behaviors
- **Scene Management**: Automatic scene loading with loading screens
- **Pause System**: Global pause functionality with UI integration
- **Event Integration**: Built-in event system integration for state changes
- **Input Handling**: Automatic input management for pause and menu navigation
- **Editor Tools**: Visual state monitoring and testing tools
- **ScriptableObject Configuration**: Easy configuration through ScriptableObjects

## Dependencies

- Unity 2022.3 or higher
- Pilipala Event System (com.pilipala.eventsystem)

## Installation

1. Add this package to your Unity project via Package Manager
2. Ensure you have the required dependencies installed
3. Create a GameStateData ScriptableObject for configuration

## Quick Start

### 1. Create Game State Data

1. Right-click in the Project window
2. Select `Create > ScriptableObjects > GameState > GameStateData`
3. Configure the state settings for different game states

### 2. Set up Game State Manager

1. Create a GameObject in your scene
2. Add the `GameStateManager` component
3. Assign your GameStateData ScriptableObject
4. Configure scene management settings

### 3. Add Input Handler (Optional)

1. Add the `GameStateInputHandler` component to handle pause and menu input
2. Configure input keys and behaviors

### 4. Configure Scenes

Add your scenes to the Build Settings:
- MainMenu
- Loading (optional)
- Game scenes

## Components

### GameStateManager

The main state controller that handles:
- State transitions
- Scene management
- Pause functionality
- Time scale control
- Cursor management

**Key Properties:**
- `StateData`: Reference to the GameStateDataSO
- `CurrentState`: Current game state
- `IsPaused`: Whether the game is paused
- `IsTransitioning`: Whether a scene transition is in progress

### GameStateInputHandler

Handles input for game state management:
- Pause/resume input
- Menu navigation
- Back button functionality

**Configuration:**
- Pause key (default: Escape)
- Menu key (default: Escape)
- Back key (default: Backspace)

## Game States

The system includes predefined states:

- **Loading**: Initial loading state
- **MainMenu**: Main menu state
- **Settings**: Settings menu state
- **LevelSelect**: Level selection state
- **Playing**: Active gameplay state
- **Paused**: Game is paused
- **LevelCompleted**: Level completed state
- **GameOver**: Game over state
- **Victory**: Victory/win state
- **Credits**: Credits/ending state
- **LoadingScreen**: Loading screen between scenes
- **Tutorial**: Tutorial state
- **Cutscene**: Cutscene state

## Events

The system triggers the following events through the EventBus:

### State Events
- `GameState.Changed` (GameState newState): When state changes
- `GameState.StateChanged` (GameState newState, GameState oldState): When state changes with previous state
- `GameState.Paused` (bool isPaused): When pause state changes

### Scene Events
- `GameState.SceneLoading` (string sceneName): When scene loading starts
- `GameState.SceneLoaded` (string sceneName): When scene loading completes
- `GameState.SceneUnloaded` (string sceneName): When scene is unloaded
- `GameState.LoadingScreenShown`: When loading screen is displayed
- `GameState.LoadingProgress` (float progress): During scene loading

### Input Events
- `GameState.PauseInput`: When pause input is detected
- `GameState.ResumeInput`: When resume input is detected
- `GameState.MenuInput`: When menu input is detected
- `GameState.BackInput`: When back input is detected

## Configuration

### GameStateDataSO

Configure state properties:
- **State Configuration**: Time scale, input, audio, UI permissions
- **Scene Management**: Target scene, loading screen, scene unloading
- **State Behavior**: Pause ability, auto-save, cursor settings
- **Events**: Custom events to trigger on state enter/exit

### StateData

Each state can be configured with:
- **Allow Time Scale**: Whether time passes in this state
- **Time Scale**: Time scale multiplier (0 = frozen, 1 = normal)
- **Allow Input**: Whether input is processed
- **Allow Audio**: Whether audio plays
- **Allow UI**: Whether UI interactions work
- **Allow Scene Transitions**: Whether scene changes are allowed
- **Can Be Paused**: Whether this state can be paused
- **Auto Save**: Whether to auto-save in this state
- **Show Cursor**: Whether cursor is visible
- **Cursor Lock Mode**: Cursor lock state

## Usage Examples

### Basic State Management

```csharp
// Get the game state manager
var gameStateManager = GameStateManager.Instance;

// Check current state
if (gameStateManager.CurrentState == GameState.Playing) {
    // Game is active
}

// Change state
gameStateManager.SetState(GameState.Paused);
```

### Event Handling

```csharp
// Subscribe to state changes
EventBusHelper.Subscribe("GameState.Changed", OnStateChanged);
EventBusHelper.Subscribe("GameState.Paused", OnPauseChanged);

private void OnStateChanged(GameState newState) {
    // Handle state change
}

private void OnPauseChanged(bool isPaused) {
    // Handle pause state
}
```

### Helper Methods

```csharp
// Use helper methods for common actions
GameStateHelper.StartGame();
GameStateHelper.PauseGame();
GameStateHelper.CompleteLevel();
GameStateHelper.GameOver();

// Check permissions
if (GameStateHelper.CanPerformAction("input")) {
    // Can process input
}
```

### Scene Management

```csharp
// Load a scene
GameStateManager.Instance.LoadScene("Level1");

// Check if transitioning
if (GameStateManager.Instance.IsTransitioning) {
    // Scene transition in progress
}
```

## Editor Tools

### Game State Monitor

Access via `Window > Game State > Game State Monitor`:
- Real-time state monitoring
- Quick state testing buttons
- Scene management tools
- Event testing capabilities
- Action permission display

### Custom Inspector

The GameStateManager component includes:
- Debug information display
- State testing buttons
- Scene testing tools
- Real-time property updates

## Customization

### Custom States

Extend the GameState enum to add custom states:

```csharp
public enum GameState
{
    // ... existing states ...
    CustomState1,
    CustomState2
}
```

### Custom State Data

Add custom properties to GameStateData:

```csharp
[Serializable]
public class GameStateData
{
    // ... existing properties ...
    [Header("Custom Properties")]
    public bool customProperty = false;
    public float customValue = 1f;
}
```

### Custom Events

Trigger custom events in state configurations:
- Set `onEnterEvent` to trigger when entering state
- Set `onExitEvent` to trigger when exiting state
- Set `onUpdateEvent` to trigger during state updates

## Game Jam Workflow

### Setup (2 minutes)
1. Create GameStateData ScriptableObject
2. Add GameStateManager to scene
3. Configure basic states (MainMenu, Playing, Paused, GameOver)

### Development (ongoing)
- Use GameStateHelper for quick state changes
- Subscribe to events for UI updates
- Use editor tools for testing

### Polish (5 minutes)
- Add loading screens
- Configure state-specific behaviors
- Test all state transitions

## Troubleshooting

### Common Issues

**State not changing:**
- Check if transitioning is in progress
- Verify state data is assigned
- Check event subscriptions

**Scene not loading:**
- Verify scene is in Build Settings
- Check scene name spelling
- Ensure scene transition is allowed

**Pause not working:**
- Check if state allows pausing
- Verify input handler is attached
- Check input key configuration

### Debug Tips

- Use the Game State Monitor window
- Check console for debug messages
- Verify event subscriptions
- Test with editor tools

## Best Practices

1. **Use Events**: Subscribe to state events rather than polling
2. **Configure States**: Set up proper state configurations for each state
3. **Test Transitions**: Use editor tools to test all state transitions
4. **Handle Edge Cases**: Consider what happens during scene transitions
5. **Use Helper Methods**: Use GameStateHelper for common operations

## Integration with Other Systems

The Game State System integrates seamlessly with:
- **Movement System**: Automatically handles input permissions
- **UI System**: Triggers UI updates on state changes
- **Audio System**: Controls audio playback based on state
- **Save System**: Triggers auto-save in appropriate states

This system provides a solid foundation for any game jam project, allowing you to focus on unique gameplay mechanics while having robust state management handled automatically. 