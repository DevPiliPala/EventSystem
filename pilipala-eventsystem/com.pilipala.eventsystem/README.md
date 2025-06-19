# EventSystem (Unity Event Bus)

A robust, type-safe event bus for Unity, designed for both programmers and designers. Includes powerful editor tools and is ready for use as a Unity package.

## How It Works
- **EventBus** lets you send and listen for events anywhere in your project.
- Supports both **type-safe (generic)** events (with payloads) and **non-generic** events (simple signals).
- Enforces type-safety: each event name can only be used as generic or non-generic, never both.
- Persistent events can be created and managed in the editor, and are available at runtime.

## For Programmers
- Subscribe to events with or without payloads:

```csharp
// Type-safe (generic) event
EventBusHelper.Subscribe<string>("gameplay.player.spawned.generic", (playerName) => { /* ... */ });
EventBusHelper.Trigger<string>("gameplay.player.spawned.generic", "Alice");

// Non-generic event
EventBusHelper.Subscribe("gameplay.player.spawned.nongeneric", () => { /* ... */ });
EventBusHelper.Trigger("gameplay.player.spawned.nongeneric");
```
- You cannot mix generic and non-generic for the same event name. The system will warn you if you try.

## For Designers
- Use the **Event Creator** (Window > Event Bus > Event Creator) to add, view, and organize events by domain and category.
- Persistent events are saved and available across sessions.
- Use the **Event Viewer** to monitor events in real time, see subscribers, and debug event activity.

## Editor Tools
- **Event Creator:** Create and manage events, grouped by domain and category. Specify if an event is generic (with payload) or non-generic.
- **Event Viewer:** See all events, their subscribers, and last triggered times. Great for debugging.
- **Inspector Integration:** Attach the `EventBusInspector` component to any GameObject to see live test results and recent events.

## Type Safety
- The system enforces that each event name is only ever used as generic (with a specific payload type) or non-generic (no payload).
- If you try to mix types for the same event name, you'll get a clear error in the console and the editor.

## Quick Start
1. Add the EventBus prefab or script to your scene (or let it auto-create).
2. Use the Event Creator to define your events.
3. Subscribe and trigger events in your scripts using `EventBusHelper`.
4. Use the Event Viewer and Inspector to monitor and debug events.

---

For more details, see the code comments and try out the included test script (`EventBusTest`).