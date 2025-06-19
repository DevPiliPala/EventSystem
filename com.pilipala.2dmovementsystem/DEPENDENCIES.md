# Dependencies Analysis

This document outlines the dependencies of the 2D Movement System package and how they have been handled.

## Core Dependencies

### 1. Unity Engine Dependencies

**Required Unity Components:**
- `Rigidbody2D` - For physics-based movement
- `BoxCollider2D` - For collision detection and ground checking
- `Transform` - For position and rotation management
- `Physics2D` - For ground detection and collision handling

**Unity Systems:**
- Unity Input System (com.unity.inputsystem) - For input handling
- Unity Physics2D - For collision detection and physics simulation

### 2. Event System Dependency

**Package Dependency:** `com.pilipala.eventsystem`

The movement system depends on your event system for:
- Movement event triggering (`Movement.Move`, `Movement.Jump`, etc.)
- State change notifications (`Movement.StateChanged`)
- Input event handling (`Movement.AimChanged`, `Movement.Crouch`)

**Event Integration:**
- Uses `EventBusHelper.Trigger()` for event firing
- Uses `EventBusHelper.Subscribe()` for event listening
- Events are string-based for flexibility

**Required Events:**
- `Movement.Move` - Triggered when movement input is detected
- `Movement.Jump` - Triggered when jump button is pressed
- `Movement.Attack` - Triggered when attack button is pressed
- `Movement.AimChanged` - Triggered when aim direction changes
- `Movement.Crouch` - Triggered when crouch state changes
- `Movement.StateChanged` - Triggered when movement state changes
- `Movement.ShootingChanged` - Triggered when shooting state changes

## Input System Dependencies

### PlayerInputActions

The system expects a `PlayerInputActions` asset with the following action maps:

**InGame Action Map:**
- `Move` - Vector2 input for movement and aiming
- `ButtonA` - Button input for jumping
- `ButtonB` - Button input for attacking
- `Select` - Button input for weapon switching
- `Start` - Button input for menu opening

**Implementation:**
- The `MovementInputHandler` implements `PlayerInputActions.IInGameActions`
- Input callbacks are handled in the `OnMove`, `OnButtonA`, `OnButtonB`, etc. methods

## ScriptableObject Dependencies

### MovementVariablesSO

Contains movement configuration data:
- Movement speeds (normal, crouch)
- Acceleration and deceleration values
- Jump force and hold time
- Aim transition speed
- Available aim angles

### MovementStateDataSO

Contains state configuration data:
- State-specific movement multipliers
- Action permissions for each state
- Invincibility durations
- State timeout values

## Layer Dependencies

### Required Layers

The system requires these layers to be set up in your project:

1. **Ground Layer** - For ground detection
   - Used in `PilipalaPlatformHandler.groundLayer`
   - Detected via `Physics2D.OverlapCircle()`

2. **Platform Layer** - For platform detection
   - Used in `PilipalaPlatformHandler.platformLayer`
   - Allows for platform-specific behavior

### Layer Setup

```csharp
// In PilipalaPlatformHandler inspector:
groundLayer = LayerMask.GetMask("Ground");
platformLayer = LayerMask.GetMask("Platform");
```

## Optional Dependencies

### Animation System

The movement system provides properties that can be used by animation systems:
- `IsGrounded` - For landing animations
- `IsMoving` - For movement animations
- `IsCrouching` - For crouch animations
- `AimDirection` - For aim animations
- `CurrentSpeed` - For speed-based animations

### Weapon System

The movement system can integrate with weapon systems through:
- `AimDirection` - For weapon aiming
- `LastHorizontalMoveDirection` - For direction-based firing
- `IsCrouching` - For crouch-based weapon behavior

## Dependency Management

### Package Dependencies

The package.json specifies these dependencies:
```json
{
  "dependencies": {
    "com.unity.inputsystem": "1.4.4",
    "com.pilipala.eventsystem": "1.0.0"
  }
}
```

### Assembly Definition

The assembly definition references:
```json
{
  "references": [
    "Unity.InputSystem",
    "PiliPala.EventSystem"
  ]
}
```

## Migration Considerations

### From Old System

When migrating from the old movement system:

1. **Event System Changes:**
   - Old: Used `EventBusHelper.Input.PlayerMove`
   - New: Uses `"Movement.Move"`

2. **ScriptableObject Changes:**
   - Old: `PlayerVariablesSO`
   - New: `MovementVariablesSO`

3. **Namespace Changes:**
   - Old: `RaccCity.Gameplay.Player`
   - New: `Pilipala.Movement`

4. **Component Changes:**
   - Old: `ContraMovementHandler`
   - New: `PilipalaPlatformHandler`

### Compatibility

The new system maintains compatibility with:
- Unity's Input System
- Your existing event system (with updated event names)
- Unity's Physics2D system
- Unity's animation system

## Testing Dependencies

To test that all dependencies are working:

1. **Event System Test:**
   ```csharp
   EventBusHelper.Subscribe("Movement.Jump", () => Debug.Log("Jump event received"));
   ```

2. **Input System Test:**
   - Verify PlayerInputActions asset is configured
   - Test input callbacks are firing

3. **Physics Test:**
   - Verify ground detection works with proper layers
   - Test collision detection

4. **ScriptableObject Test:**
   - Verify MovementVariablesSO can be created and assigned
   - Test state data configuration

## Troubleshooting Dependencies

### Common Issues

1. **Missing Event System:**
   - Error: "The type or namespace name 'EventBusHelper' could not be found"
   - Solution: Install `com.pilipala.eventsystem` package

2. **Missing Input System:**
   - Error: "The type or namespace name 'PlayerInputActions' could not be found"
   - Solution: Install Unity Input System package

3. **Missing Layers:**
   - Warning: "Ground detection not working"
   - Solution: Set up Ground and Platform layers

4. **Missing ScriptableObjects:**
   - Error: "MovementVariables not assigned"
   - Solution: Create and assign MovementVariablesSO assets

### Dependency Resolution

If you encounter dependency issues:

1. Check Package Manager for missing packages
2. Verify assembly definition references
3. Ensure all required layers are set up
4. Confirm ScriptableObject assets are created and assigned 