# 2D Movement System

A flexible 2D movement system for Unity with support for platformer-style movement, aiming, and state management.

## Features

- **Pilipala Platform Movement**: 8-directional aiming with smooth movement
- **State Management**: Built-in state system for different movement states (Normal, Shooting, Prone, Jumping, etc.)
- **Variable Jump Height**: Hold jump button for variable jump height
- **Crouching System**: Smooth collider transitions when crouching
- **Event Integration**: Built-in event system integration for movement events
- **Input System Support**: Works with Unity's new Input System
- **ScriptableObject Configuration**: Easy configuration through ScriptableObjects

## Dependencies

- Unity 2022.3 or higher
- Unity Input System (com.unity.inputsystem)
- Pilipala Event System (com.pilipala.eventsystem)

## Installation

1. Add this package to your Unity project via Package Manager
2. Ensure you have the required dependencies installed
3. Set up your Input Actions asset for the PlayerInputActions

## Quick Start

### 1. Create Movement Variables

1. Right-click in the Project window
2. Select `Create > ScriptableObjects > Movement > MovementVariables`
3. Configure the movement parameters (speed, acceleration, jump force, etc.)

### 2. Create State Data

1. Right-click in the Project window
2. Select `Create > ScriptableObjects > Movement > MovementStateData`
3. Configure the state settings for different movement states

### 3. Set up GameObject

1. Create a GameObject with Rigidbody2D and BoxCollider2D
2. Add the `PilipalaPlatformHandler` component
3. Add the `MovementStateManager` component
4. Add the `MovementInputHandler` component
5. Assign your MovementVariables and MovementStateData ScriptableObjects

### 4. Configure Layers

Set up the following layers in your project:
- Ground Layer: For ground detection
- Platform Layer: For platform detection

Assign these layers to the respective fields in the PilipalaPlatformHandler component.

## Components

### PilipalaPlatformHandler

The main movement controller that handles:
- Ground detection
- Movement physics
- Jump mechanics
- Aim direction snapping
- Collider transitions for crouching

**Key Properties:**
- `MovementVariables`: Reference to the MovementVariablesSO
- `IsGrounded`: Whether the entity is on the ground
- `IsMoving`: Whether the entity is currently moving
- `AimDirection`: Current aim direction (8-way snapped)
- `MoveDirection`: Current movement direction

### MovementStateManager

Manages different movement states and their properties:
- State transitions
- Action permissions
- State timeouts
- Invincibility frames

**States:**
- Normal: Default state
- Shooting: While firing weapon
- Prone: While crouching
- Jumping: While in air
- Invincible: After taking damage
- Dead: When health reaches 0

### MovementInputHandler

Handles input processing and movement commands:
- Input system integration
- Button hold detection
- Event triggering
- Aim input processing

## Events

The system triggers the following events through the EventBus:

- `Movement.Move`: When movement input is detected
- `Movement.Jump`: When jump button is pressed
- `Movement.Attack`: When attack button is pressed
- `Movement.AimChanged`: When aim direction changes
- `Movement.Crouch`: When crouch state changes
- `Movement.StateChanged`: When movement state changes
- `Movement.ShootingChanged`: When shooting state changes

## Configuration

### MovementVariablesSO

Configure movement parameters:
- **Move Speed**: Base movement speed
- **Crouch Move Speed**: Speed while crouching
- **Acceleration**: How quickly speed is reached
- **Deceleration**: How quickly speed is lost
- **Max Speed**: Maximum movement speed
- **Jump Force**: Initial jump force
- **Jump Hold Time**: Maximum time jump can be held

### StateData

Configure state properties:
- **Move Speed Multiplier**: Speed multiplier for this state
- **Can Move**: Whether movement is allowed
- **Can Jump**: Whether jumping is allowed
- **Can Shoot**: Whether shooting is allowed
- **Can Aim**: Whether aiming is allowed
- **Can Crouch**: Whether crouching is allowed
- **Invincibility Duration**: Invincibility time for this state

## Usage Examples

### Basic Movement Setup

```csharp
// Get the movement handler
var movementHandler = GetComponent<PilipalaPlatformHandler>();

// Check if grounded
if (movementHandler.IsGrounded) {
    // Can jump
}

// Get current aim direction
Vector2 aimDir = movementHandler.AimDirection;
```

### State Management

```csharp
// Get the state manager
var stateManager = GetComponent<MovementStateManager>();

// Check if can perform action
if (stateManager.CanPerformAction(MovementAction.Jump)) {
    // Can jump
}

// Set state
stateManager.SetState(MovementState.Shooting);
```

### Event Handling

```csharp
// Subscribe to movement events
EventBusHelper.Subscribe("Movement.Jump", OnJump);
EventBusHelper.Subscribe("Movement.StateChanged", OnStateChanged);

private void OnJump() {
    // Handle jump event
}

private void OnStateChanged(MovementState newState) {
    // Handle state change
}
```

## Customization

### Custom Movement Variables

You can create custom MovementVariablesSO instances for different entities (players, enemies, etc.) with different movement characteristics.

### Custom States

Extend the MovementState enum and StateData to add custom states for your specific needs.

### Custom Input Handling

You can create custom input handlers by implementing the IAimInputProvider interface and handling input differently.

## Troubleshooting

### Common Issues

1. **Movement not working**: Check that MovementVariables is assigned
2. **No ground detection**: Verify ground and platform layers are set correctly
3. **Input not responding**: Ensure Input Actions asset is properly configured
4. **Events not firing**: Check that EventBus is available in the scene

### Debug Information

The system provides debug warnings when:
- MovementVariables is not assigned
- StateData is missing for a state
- Required components are missing

## License

MIT License - see LICENSE file for details.

## Support

For issues and questions, please refer to the project repository or contact the maintainer. 