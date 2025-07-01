# 2D Movement System

Handles 2D platformer movement so you don't have to write it from scratch. Move, jump, crouch, aim - all the basics covered.

## What's this for?

Attempts to recreate basic platformer movement:
- WASD/arrow key movement
- Jump with variable height (hold longer = jump higher)
- 8-direction aiming (snaps to clean angles)
- Crouching that shrinks the collider
- Ground detection that actually works
- States for different movement modes (shooting, prone, etc.)

Basically all the movement code you'd write anyway, but already done and tested.

## What you need

- Unity 2022.3+
- Unity's Input System package
- Pilipala Event System (from this same repo)

## Setup

### 1. Make config files
- Right-click → Create → ScriptableObjects → Movement → MovementVariables
- Right-click → Create → ScriptableObjects → Movement → MovementStateData
- Tweak the settings (speed, jump height, etc.)

### 2. Set up your player
- GameObject with Rigidbody2D and BoxCollider2D
- Add `PilipalaPlatformHandler` component
- Add `MovementStateManager` component  
- Add `MovementInputHandler` component
- Drag your config files to the components

### 3. Set up layers
- Make a "Ground" layer for floors/walls
- Make a "Platform" layer for jump-through platforms
- Set these in the PilipalaPlatformHandler component

That's it. Your character should move around now.

## The main components

### PilipalaPlatformHandler
This is the brain that handles:
- Moving around
- Jumping with variable height
- Ground detection
- 8-way aim snapping
- Collider resizing when crouching

### MovementStateManager
Manages what the character can do in different states:
- Normal: Can do everything
- Shooting: Maybe can't move while shooting
- Prone: Crouched, slower movement
- Jumping: In the air
- Invincible: After taking damage
- Dead: Game over

### MovementInputHandler
Takes input and tells the other components what to do. Handles the new Input System stuff so you don't have to.

## Events you can listen for

```csharp
// Listen for movement events
EventBusHelper.Subscribe("Movement.Jump", OnJump);
EventBusHelper.Subscribe("Movement.StateChanged", OnStateChanged);
EventBusHelper.Subscribe("Movement.AimChanged", OnAimChanged);
```

Events fired:
- `Movement.Move` - Player moved
- `Movement.Jump` - Player jumped
- `Movement.Attack` - Player attacked
- `Movement.AimChanged` - Aim direction changed
- `Movement.Crouch` - Crouch state changed
- `Movement.StateChanged` - Movement state changed

## Code examples

```csharp
// Check if player is on the ground
var movement = GetComponent<PilipalaPlatformHandler>();
if (movement.IsGrounded) {
    // Can jump
}

// Get where player is aiming (snapped to 8 directions)
Vector2 aimDirection = movement.AimDirection;

// Check if player can do something in current state
var stateManager = GetComponent<MovementStateManager>();
if (stateManager.CanPerformAction(MovementAction.Jump)) {
    // Jump is allowed in current state
}

// Change movement state
stateManager.SetState(MovementState.Shooting);
```

## Common issues

**Player won't move:** Check that MovementVariables is assigned to the PilipalaPlatformHandler.

**No jumping:** Make sure ground/platform layers are set up correctly. The player needs to detect ground to jump.

**Input not working:** Check that you have an Input Actions asset set up with the right action names.

**Weird physics:** Check that your Rigidbody2D has Freeze Rotation Z checked (unless you want the player spinning around).

## Customization

You can make different MovementVariables files for different characters - fast ninja vs heavy tank, etc.

You can also extend the MovementState enum if you need custom states like "Swimming" or "Climbing".

Works great with the other Pilipala packages - the Event System handles all the communication between components. 