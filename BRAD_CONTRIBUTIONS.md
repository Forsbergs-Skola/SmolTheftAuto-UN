# Project Contributions

## Overview

This document outlines my contributions to the SmolTheftAuto-UN project. My primary focus was on developing the backend systems architecture and implementing a comprehensive UI framework. The work emphasizes modularity, maintainability, and a decoupled event-driven architecture that allows team members to work independently while maintaining system cohesion.

---

## 1. Event Channel System (Primary Focus)

### Location
`Assets/Tools/EventSystem/`

### Description
I designed and implemented a comprehensive ScriptableObject-based event channel system that serves as the backbone of inter-system communication throughout the entire project. This system has become extensively used across all game systems, enabling loose coupling between components.

### Key Features

#### Event Payload Types
The system supports multiple payload types to accommodate different event scenarios:

- **EmptyPayloadEvent** - For events without data (e.g., pause toggled, dialogue advanced)
- **IntPayloadEvent** - For numeric events (e.g., money changed, health updated)
- **BoolPayloadEvent** - For boolean state changes (e.g., save exists, sprint active)
- **StringPayloadEvent** - For text-based events (e.g., dialogue started/ended with conversation name)
- **FloatPayloadEvent** - For floating-point values
- **Vector2PayloadEvent** - For 2D vector events (e.g., movement input)
- **Vector3PayloadEvent** - For 3D vector events
- **EnumQuestPayloadEvent** - For quest-related events
- **EnumWeaponPayloadEvent** - For weapon system events
- **GameObjectPayloadEvent** - For GameObject references
- **TransformPayloadEvent** - For Transform references
- **ColliderPayloadEvent** - For collision events

#### Architecture Benefits
1. **Decoupled Systems**: Components can communicate without direct references
2. **Editor-Friendly**: Events are created as ScriptableObjects, allowing designers to wire up systems visually
3. **Type Safety**: Strong typing prevents runtime errors
4. **Testability**: Events can be triggered and monitored independently
5. **Extensibility**: New payload types can be easily added following the established pattern

#### Implementation Pattern
Each event type follows a consistent pattern:

```csharp
[CreateAssetMenu(fileName = "EventName", menuName = "Event Channels/...")]
public class PayloadEvent : ScriptableObject
{
    public event System.Action<T> OnEventTriggered;
    public void TriggerEvent(T payload)
    {
        OnEventTriggered?.Invoke(payload);
    }
}
```

### Usage Throughout Project
The event system is used extensively in:
- Game state management (pause, save/load)
- Player data updates (health, money, ammo)
- Quest system (start/complete quests)
- Dialogue system (start/end conversations)
- Store interactions
- Input handling
- UI updates

---

## 2. Game Manager Singleton

### Location
`Assets/Scripts/GameManagerSingleton.cs`

### Description
I implemented a comprehensive singleton game manager that serves as the central authority for game state, player data, and system coordination. This manager acts as the primary orchestrator, listening to events from various systems and maintaining the authoritative state of the game.

### Key Responsibilities

#### Player Data Management
- Maintains current player state (health, money, ammo, inventory, quest items)
- Enforces resource limits (max health, max money, max grenades)
- Manages weapon ammo tracking (in-clip vs. total ammo for rifle, pistol, shotgun)
- Tracks game progress (checkpoints reached, NPCs killed)

#### Quest System Integration
- Tracks quest start/completion states
- Validates quest completion criteria (e.g., 3 checkpoints for gas can quest)
- Manages quest state persistence through save system
- Coordinates quest UI updates

#### Event Coordination
The manager subscribes to and handles 20+ different event types:
- Player resource events (money, health, ammo, grenades)
- Quest events (start, complete)
- Dialogue events (start, end, advance)
- Store interaction events
- Save/load events
- Gameplay events (checkpoint reached, NPC killed, weapon equipped/reloaded)

#### Scene Management
- Handles scene transitions between menu and gameplay
- Manages game initialization and reset logic
- Coordinates loading screen display

#### Configuration
Centralized configuration for:
- Resource limits and capacities
- Store pricing
- Ammo pickup values
- Clip capacities per weapon type

### Design Patterns Used
- **Singleton Pattern**: Ensures single instance across scenes
- **Observer Pattern**: Listens to events from multiple systems
- **State Management**: Maintains authoritative game state

### Lessons Learned

When I first implemented the GameManagerSingleton, I understood the theoretical concept of the Singleton pattern—ensuring a single instance exists across the entire application, but I was not yet fluent enough in the Unity idiom to implement it properly. Coming from a background primarily in Godot and GDScript, I found myself working with Unity's C# API for the first time in a substantial project.

Looking at the `Awake()` method implementation:

```110:115:Assets/Scripts/GameManagerSingleton.cs
private void Awake()
{
    if (GameObject.FindGameObjectsWithTag(Constants.Tags.GAME_MANAGER).Length > 0) { Destroy(gameObject); }
    tag = Constants.Tags.GAME_MANAGER;
    DontDestroyOnLoad(gameObject);
}
```

This implementation demonstrates my approach at the time: I understood the high-level functionality needed (ensuring uniqueness, persistence across scenes) and improvised a solution using the Unity concepts I was comfortable with: GameObject tags and the `FindGameObjectsWithTag` method. This achieved the goal of singleton behavior through tag-based lookup and instance destruction, rather than the more conventional Unity singleton pattern that uses static instance references and direct self-reference checks.

Since implementing this, I have done additional research and practice with Unity-specific patterns and now understand the more idiomatic approaches to implementing singletons in Unity (typically using static instance references with null checks). However, given that this implementation works correctly and is deeply integrated throughout the project at this stage, and considering the project timeline constraints, I have chosen to leave the code as-is rather than perform a major refactoring that would touch many dependent systems.

This experience highlights the importance of learning framework-specific idioms and patterns, while also demonstrating that functional solutions can be achieved through different approaches when working within one's current knowledge constraints. It has been a valuable lesson in balancing theoretical understanding with practical implementation, and recognizing when to prioritize stability over optimization in an active project.

---

## 3. User Interface System

### Location
`Assets/Scripts/UIScripts/`

### Description
I designed and implemented a complete UI management system that handles all user interfaces in the game. The system is built around a centralized canvas manager that orchestrates multiple specialized UI canvases.

### Canvas Manager (`CanvasManager.cs`)

The Canvas Manager serves as the central hub for UI management:

#### Features
- **Unified Canvas Control**: Manages all UI canvases through a single interface
- **Canvas State Tracking**: Maintains current and previous active canvas states
- **Automatic Cleanup**: Automatically hides inactive canvases
- **Sorting Order Management**: Handles canvas layering for overlay scenarios
- **Cursor Management**: Automatically shows/hides cursor based on active canvas
- **Loading Screen Integration**: Coordinates fade effects during scene transitions

#### Canvas Interface Pattern
All canvases implement the `ICanvasable` interface, ensuring consistent behavior:
```csharp
public interface ICanvasable
{
    EnumCanvasName CanvasName();
    GameObject GetCanvasObject();
    bool GetIsVisible();
    void SetIsVisible(bool val);
}
```

#### Canvas Types Managed
- **HUD Canvas** - In-game player information
- **Pause Canvas** - Pause menu with quest and inventory panels
- **Main Menu Canvas** - Main menu with new game/continue options
- **Dialogue Canvas** - Conversation display system
- **Store Canvas** - Shop interaction interface
- **Loading Canvas** - Scene transition screen

### Individual Canvas Implementations

#### HUD Canvas (`HudCanvas.cs`)
Real-time player information display:
- Health bar (slider)
- Current weapon display
- Ammo counter (in-clip / total)
- Money display
- Quest progress indicators (checkpoints, NPC kills)
- Mission passed effect with animated text
- Player death effect with main menu return option

#### Dialogue Canvas (`DialogueCanvas.cs`)
Sophisticated dialogue system:
- Character name display
- Left/right portrait system with texture loading
- Portrait subdue effect (grey out inactive speaker)
- Character-by-character text reveal animation
- Configurable reveal interval
- Conversation ScriptableObject system (`Conversation.cs`, `DialogueLine.cs`)
- Event-driven dialogue flow
- Resource cleanup on dialogue end

#### Pause Canvas (`PauseCanvas.cs`)
Pause menu with dual panel system:
- **Quest Panel**: Dynamic quest list display
  - Quest items spawned programmatically
  - Quest text strikethrough on completion
  - Quest text positioning system
  - Quest state persistence
- **Inventory Panel**: Player inventory display
  - Visual indicators for quest items (gas can, matches, sunglasses)
- Panel switching functionality
- Save game feedback animation

#### Main Menu Canvas (`MainMenuCanvas.cs`)
Main menu interface:
- New Game button (clears save if exists)
- Continue button (only shown when save exists)
- Dynamic button visibility based on save state
- Event-driven button actions
- About and Quit functionality

#### Store Canvas (`StoreCanvas.cs`)
Shop interaction system:
- Current player stats display (money, ammo counts)
- Dynamic UI updates based on player funds
- Color-coded feedback (green when affordable, red when broke)
- Ammo refill functionality
- Integration with player data system

#### Loading Canvas (`LoadcingCanvas.cs`)
Scene transition screen:
- Black fade overlay
- Alpha animation control
- Coordinated with scene loading

#### Quest Panel (`QuestPanel.cs`)
Dynamic quest management:
- Programmatic quest item instantiation
- Quest positioning algorithm (spacing-based layout)
- Quest state persistence
- Quest completion visualization (strikethrough)
- Quest initialization from save data

#### Quest Item (`QuestItem.cs`)
Individual quest UI element:
- Quest name and description display
- Strikethrough text effect for completed quests
- Color change on completion (grey)

---

## 4. Save System

### Location
`Assets/Scripts/Data/SaveData.cs` and `SaveManager.cs`

### Description
I implemented a comprehensive save/load system that persists game state to disk using Unity's JSON serialization.

### Save Data Model (`SaveData.cs`)
Serializable data structure containing:
- Player health and resources
- Weapon ammo (rifle, pistol, shotgun - total and in-clip)
- Grenades and money
- Quest progress (checkpoints, NPCs killed)
- Quest items collected (gas can, matches, sunglasses)
- Quest start/completion states
- Equipped weapon
- Player position (prepared for future use)

### Save Manager (`SaveManager.cs`)
Save system implementation:

#### Data Structures
- **PlayerData** - Runtime player state struct
- **QuestStartedData** - Runtime quest state struct
- **SaveData** - Serializable save format

#### Features
- **Save API**: `Save(PlayerData, QuestStartedData)` - Converts runtime data to serializable format
- **Load API**: `Load()` - Deserializes save file to SaveData
- **Clear API**: `Clear()` - Deletes save file
- **Exists Check**: `SaveExists()` - Checks for save file presence

#### Data Conversion
The manager handles conversion between:
- Runtime structs (`PlayerData`, `QuestStartedData`) ↔ Serializable class (`SaveData`)
- Enum to string conversion for weapon types
- String to enum conversion on load

#### File Management
- Saves to `Application.persistentDataPath/save.json`
- Pretty-printed JSON for readability
- Automatic file existence checking

#### Integration
- Fully integrated with GameManagerSingleton
- Event-driven save/load triggers
- UI updates on save state changes

---

## 5. Supporting Tools

### Location
`Assets/Tools/`

### Input Handler (`InputHandler.cs`)
Unified input management system:
- Processes keyboard and gamepad input
- Converts input to events using the event channel system
- Input cooldown system to prevent rapid-fire triggers
- Scene-aware pause handling
- Singleton pattern for cross-scene persistence

### Tween Service (`TweenService/`)
Animation service system:
- Generic tween system for smooth animations
- Multiple easing functions (Linear, Quad, Cubic, Quart, Sine)
- Ease directions (In, Out, In-Out)
- Support for float, Vector2, and Vector3 tweening
- Event-driven callbacks (OnValueUpdated, OnFinished)
- Used extensively in UI animations (fade effects, scaling, etc.)

### Constants (`Helpers/Constants.cs`)
Centralized constant definitions:
- Tag constants for consistent GameObject tagging
- Namespaced in `GameTools` for organization
- Used throughout project for tag-based lookups

### State Machine System (`StateMachine/`)
ScriptableObject-based state machine:
- States and transitions as ScriptableObjects
- Flexible state management system
- Used for player and enemy AI state management

---

## 6. Bootstrap Scene

### Location
`Assets/Scenes/Bootstrap.unity`

### Description
I set up the Bootstrap scene as the initialization point for the game. This scene contains all persistent systems that need to exist across scene loads:

- GameManagerSingleton instance
- CanvasManager with all UI canvases
- InputHandler instance
- SaveManager instance
- UI Event System

The Bootstrap scene ensures that critical systems are initialized before any gameplay begins and persist across scene transitions, providing a stable foundation for the entire game.

---

## Technical Highlights

### Architecture Patterns
1. **Event-Driven Architecture**: Heavy reliance on event channels for decoupled communication
2. **Singleton Pattern**: Used for managers that need global access
3. **ScriptableObject Pattern**: Events, conversations, and states as data assets
4. **Interface-Based Design**: `ICanvasable` interface for consistent canvas behavior
5. **Separation of Concerns**: Clear boundaries between UI, game logic, and data

### Code Quality
- Consistent naming conventions
- Comprehensive comments and documentation
- Type safety with explicit typing
- Error handling and validation
- Resource cleanup (textures, GameObjects)

### Extensibility
- Easy to add new event types
- Simple to create new UI canvases
- Modular save data structure
- Flexible quest system

### Performance Considerations
- Efficient event subscription/unsubscription
- Object pooling ready (quest items)
- Resource unloading (dialogue textures)
- Minimal Update() loops

---

## Impact on Project

### Team Collaboration
The event channel system has enabled team members to work independently:
- Systems can be developed in parallel without direct dependencies
- UI updates automatically through events
- New features can be added by simply creating/using events

### Maintainability
- Centralized game state management
- Clear separation between UI and game logic
- Consistent patterns throughout codebase
- Easy to debug through event tracing

### Scalability
- Easy to add new UI screens
- Simple to extend save system
- Flexible quest addition
- Expandable event system

---

## Summary

My contributions focus on establishing a robust, maintainable foundation for the game through:
1. **Comprehensive event system** that decouples all game systems
2. **Centralized game management** that maintains authoritative state
3. **Complete UI framework** with sophisticated canvas management
4. **Persistent save system** for game state preservation
5. **Supporting tools** that enhance development workflow

These systems have become integral to the project's architecture and are used extensively by all team members, demonstrating their effectiveness and value to the overall project success.

