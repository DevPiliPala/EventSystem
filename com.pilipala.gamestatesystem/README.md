# Game State System

Simple game state manager for Unity. Handles all the boring stuff like pausing, scene loading, and state transitions so you don't have to.

## What's this for?

This handles stuff like:
- Pausing the game
- Switching between menus and gameplay
- Loading screens between levels
- Managing what can happen in each state (like blocking input during cutscenes)

This package does all that automatically. Just tell it what state you want and it handles the rest.

## What you need

- Unity 2022.3+
- The Pilipala Event System package (it's in this same repo)

## Setup (takes like 2 minutes)

1. Drop this package into your project
2. Make sure you have the event system package too
3. Right-click in project → Create → ScriptableObjects → GameState → GameStateData
4. Done

## How to use it

### Basic setup

1. **Make a config file**
   - Right-click → Create → ScriptableObjects → GameState → GameStateData
   - This is where you configure what each state does

2. **Add the manager to your scene**
   - Create empty GameObject, add `GameStateManager` component
   - Drag your config file to the State Data field
   - That's it, you're done

3. **Optional: Add input handling**
   - Add `GameStateInputHandler` component anywhere
   - Handles Esc to pause, backspace to go back, etc.

### States you get for free

The system comes with these states built-in:
- `Loading` - Game is starting up
- `MainMenu` - Self explanatory
- `Settings` - Settings screen
- `LevelSelect` - Level picker
- `Playing` - Actually playing the game
- `Paused` - Game is paused
- `LevelCompleted` - Just finished a level
- `GameOver` - Player died/failed
- `Victory` - Player won
- `Credits` - End credits
- `LoadingScreen` - Between scenes
- `Tutorial` - Tutorial sections
- `Cutscene` - Story scenes

## The two main components

### GameStateManager
This is the brain. It handles:
- Switching between states
- Loading scenes automatically
- Pausing/unpausing
- Managing time scale
- Cursor visibility

Just call `GameStateManager.Instance.SetState(GameState.Paused)` and it does everything.

### GameStateInputHandler
Handles keyboard input:
- Esc key pauses/unpauses
- Backspace goes back to previous state
- Configurable in the inspector

## Events (if you want to listen for stuff)

The system fires events when things happen. If you want to react to state changes:

```csharp
// Listen for any state change
EventBusHelper.Subscribe("GameState.Changed", OnStateChanged);

// Listen for pause changes
EventBusHelper.Subscribe("GameState.Paused", OnPauseChanged);

// Listen for scene loading
EventBusHelper.Subscribe("GameState.SceneLoading", OnSceneLoading);
```

All the events:
- `GameState.Changed` - State changed
- `GameState.Paused` - Pause state changed
- `GameState.SceneLoading` - Scene started loading
- `GameState.SceneLoaded` - Scene finished loading
- `GameState.PauseInput` - Player pressed pause
- etc.

## Code examples

```csharp
// Change state (the easy way)
GameStateHelper.StartGame();      // Goes to Playing state
GameStateHelper.PauseGame();      // Goes to Paused state  
GameStateHelper.CompleteLevel();  // Goes to LevelCompleted state
GameStateHelper.GameOver();       // Goes to GameOver state

// Change state (the direct way)
GameStateManager.Instance.SetState(GameState.MainMenu);

// Check current state
if (GameStateManager.Instance.CurrentState == GameState.Playing) {
    // Do gameplay stuff
}

// Check if paused
if (GameStateManager.Instance.IsPaused) {
    // Show pause menu or whatever
}

// Load a scene
GameStateManager.Instance.LoadScene("Level1");
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