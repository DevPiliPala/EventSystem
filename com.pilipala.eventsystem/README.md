# EventSystem

Simple event bus for Unity. Send messages between scripts without them needing to know about each other.

## What's this for?

You know how sometimes you need Script A to tell Script B that something happened, but you don't want them directly connected? This does that.

Like when the player dies, you want to:
- Update the UI
- Play a death sound  
- Save the score
- Show game over screen

Instead of the player script knowing about all those systems, it just fires a "player died" event and everyone who cares can listen for it.

## How it works
- Send events with `EventBusHelper.Trigger("event.name")`
- Listen for events with `EventBusHelper.Subscribe("event.name", MyFunction)`
- Events can have data attached or just be simple signals
- Type-safe so you can't accidentally send the wrong data type

## Code examples

```csharp
// Simple event (no data)
EventBusHelper.Subscribe("player.died", OnPlayerDied);
EventBusHelper.Trigger("player.died");

// Event with data
EventBusHelper.Subscribe<int>("player.score.changed", OnScoreChanged);
EventBusHelper.Trigger<int>("player.score.changed", 1000);

// Event with multiple data types
EventBusHelper.Subscribe<string, int>("player.levelup", OnLevelUp);
EventBusHelper.Trigger<string, int>("player.levelup", "Warrior", 5);

void OnPlayerDied() {
    // Show game over screen
}

void OnScoreChanged(int newScore) {
    // Update UI
}

void OnLevelUp(string className, int newLevel) {
    // Show level up effects
}
```

**Important:** You can't mix data types for the same event name. If you use `"player.died"` as a simple event, you can't later use it with data. The system will yell at you if you try.

## Editor tools (pretty handy)

### Event Creator
- Window → Event Bus → Event Creator
- Create events ahead of time so you don't have typos
- Organize them by category (like "gameplay", "ui", "audio")
- Specify if they carry data or not

### Event Viewer  
- Window → Event Bus → Event Viewer
- See all events firing in real-time
- Great for debugging when something isn't working
- Shows who's listening to what

### Inspector
- Add `EventBusInspector` component to any GameObject
- Shows recent events and lets you test firing events manually

## Setup

Just add this package to your project. The EventBus will create itself automatically when you first use it.

If you want to be fancy, you can create events in the Event Creator first, but you don't have to.