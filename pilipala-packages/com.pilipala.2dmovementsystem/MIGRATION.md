# Migration Guide

This guide helps you migrate from the old movement system to the new `com.pilipala.2dmovementsystem` package.

## Overview of Changes

The movement system has been extracted into its own package with the following changes:

- **Namespace Changes**: All classes now use `Pilipala.Movement` namespace
- **Dependency Updates**: Now depends on `com.pilipala.eventsystem` package
- **Component Renames**: Some components have been renamed for clarity
- **Event System Integration**: Updated to use the new event system

## Step-by-Step Migration

### 1. Install the Package

1. Add the `com.pilipala.2dmovementsystem` package to your project
2. Ensure you have the `com.pilipala.eventsystem` package installed
3. Remove the old movement-related scripts from your project

### 2. Update Script References

#### Old → New Class Names

| Old Class | New Class | Location |
|-----------|-----------|----------|
| `ContraMovementHandler` | `Pilipala.Movement.PilipalaPlatformHandler` | `Assets/Code/Gameplay/Player/` → Package |
| `PlayerStateManager` | `Pilipala.Movement.MovementStateManager` | `Assets/Code/Gameplay/Player/` → Package |
| `ContraPlayerInputHandler` | `Pilipala.Movement.Input.MovementInputHandler` | `Assets/Code/Input/InputHandlers/` → Package |
| `PlayerVariablesSO` | `Pilipala.Movement.Data.MovementVariablesSO` | `Assets/Code/Core/Data/` → Package |
| `PlayerStateDataSO` | `Pilipala.Movement.Data.MovementStateDataSO` | `Assets/Code/Core/Data/` → Package |
| `StateData` | `Pilipala.Movement.Data.StateData` | `Assets/Code/Core/Data/` → Package |
| `PlayerState` | `Pilipala.Movement.Data.MovementState` | `Assets/Code/Gameplay/Player/` → Package |
| `IMoveHandler` | `Pilipala.Movement.Interfaces.IMoveHandler` | `Assets/Code/Core/Interfaces/` → Package |
| `IAimInputProvider` | `Pilipala.Movement.Interfaces.IAimInputProvider` | `Assets/Code/Core/Interfaces/` → Package |

### 3. Update Using Statements

Replace old using statements with new ones:

```csharp
// Old
using RaccCity.Core.Interfaces;
using RaccCity.Core.Data;
using RaccCity.Gameplay.Player;
using RaccCity.Core.EventBus;

// New
using Pilipala.Movement.Interfaces;
using Pilipala.Movement.Data;
using Pilipala.Movement;
using EventSystem;
```

### 4. Update Component References

#### On GameObjects

1. **ContraMovementHandler** → **PilipalaPlatformHandler**: 
   - Rename the component
   - Update the script reference
2. **PlayerStateManager** → **MovementStateManager**: 
   - Rename the component
   - Update the ScriptableObject reference to use `MovementStateDataSO`
3. **ContraPlayerInputHandler** → **MovementInputHandler**:
   - Rename the component
   - Update the ScriptableObject reference to use `MovementVariablesSO`

#### In Scripts

```csharp
// Old
[SerializeField] private PlayerStateManager stateManager;
[SerializeField] private PlayerVariablesSO playerVariables;

// New
[SerializeField] private MovementStateManager stateManager;
[SerializeField] private MovementVariablesSO movementVariables;
```

### 5. Update Event References

The event system has been updated to use the new event system:

```csharp
// Old
EventBusHelper.Trigger(EventBusHelper.Input.PlayerMove);
EventBusHelper.Trigger(EventBusHelper.Input.PlayerJump);

// New
EventBusHelper.Trigger("Movement.Move");
EventBusHelper.Trigger("Movement.Jump");
```

#### Event Name Mapping

| Old Event | New Event |
|-----------|-----------|
| `EventBusHelper.Input.PlayerMove` | `"Movement.Move"` |
| `EventBusHelper.Input.PlayerJump` | `"Movement.Jump"` |
| `EventBusHelper.Input.PlayerAttack` | `"Movement.Attack"` |
| `EventBusHelper.Input.PlayerAimChanged` | `"Movement.AimChanged"` |
| `EventBusHelper.Input.PlayerCrouch` | `"Movement.Crouch"` |
| `EventBusHelper.Gameplay.PlayerStateChanged` | `"Movement.StateChanged"` |
| `EventBusHelper.Gameplay.PlayerShootingChanged` | `"Movement.ShootingChanged"` |

### 6. Update ScriptableObject References

#### PlayerVariablesSO → MovementVariablesSO

1. Create new `MovementVariablesSO` assets
2. Copy values from old `PlayerVariablesSO` assets
3. Update component references

#### PlayerStateDataSO → MovementStateDataSO

1. Create new `MovementStateDataSO` assets
2. Copy state configurations from old assets
3. Update component references

### 7. Update Animation Controllers

If you have animation controllers that reference the old movement system:

```csharp
// Old
Vector2 snappedDir = ContraMovementHandler.SnapTo8Directions(rawAim);

// New
Vector2 snappedDir = Pilipala.Movement.PilipalaPlatformHandler.SnapTo8Directions(rawAim);
```

### 8. Update Enemy Movement

If you have enemies using the movement system:

```csharp
// Old
using RaccCity.Core.Data;

// New
using Pilipala.Movement.Data;

// Update variable references
[SerializeField] protected EnemyVariablesSO enemyVariables;
// Consider creating MovementVariablesSO for enemies as well
```

## Breaking Changes

### 1. Namespace Changes

All movement-related classes are now in the `Pilipala.Movement` namespace.

### 2. Event System Changes

Events now use string-based names instead of the old EventBusHelper constants.

### 3. Component Dependencies

The `MovementInputHandler` now requires a `MovementVariablesSO` instead of `PlayerVariablesSO`.

### 4. State Management

The state system has been separated from the player-specific implementation and is now generic.

## Testing Your Migration

1. **Check Console**: Look for any compilation errors or missing references
2. **Test Movement**: Ensure basic movement, jumping, and aiming work
3. **Test Events**: Verify that movement events are firing correctly
4. **Test States**: Ensure state transitions work as expected
5. **Test Input**: Verify that input handling works with your Input Actions

## Rollback Plan

If you encounter issues:

1. Keep a backup of your old movement scripts
2. You can temporarily revert by:
   - Removing the package
   - Restoring the old scripts
   - Updating the using statements back to the old namespaces

## Support

If you encounter issues during migration:

1. Check the console for specific error messages
2. Verify all dependencies are installed correctly
3. Ensure ScriptableObject references are updated
4. Check that the event system is properly configured

## Post-Migration Cleanup

After successful migration:

1. Remove old movement scripts from your project
2. Clean up any unused ScriptableObject assets
3. Update any documentation or comments referencing the old system
4. Test thoroughly to ensure all functionality works as expected 