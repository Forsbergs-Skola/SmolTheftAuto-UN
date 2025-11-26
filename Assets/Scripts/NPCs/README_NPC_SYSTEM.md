# NPC System Documentation - Smol Theft Auto

## Overview
This NPC system has been redesigned following modern Unity patterns with component-based architecture, event-driven communication, and decoupled systems. It integrates seamlessly with existing project tools and follows all design patterns established in the codebase.

## Architecture

### Component-Based Design
The NPC system uses a component-based architecture where each component handles a single responsibility:

- **NPCController**: Main coordinator that references all components
- **NPCHealth**: Health management, implements `IDamageable`
- **NPCCombat**: Deals damage to player at intervals
- **NPCMovement**: Handles patrol/wander behavior using NavMesh
- **NPCLoot**: Drops money when NPC dies
- **NPCVehicleCollision**: Handles vehicle collision damage

### Manager System
- **NPCManager**: Centralized manager for spawning, tracking, and respawning NPCs
  - Implements `INPCSpawner` interface
  - Uses ServiceLocator pattern for decoupled access
  - Supports save/load functionality (ISaveable interface)

### Data Configuration
- **NPCData**: ScriptableObject for configurable NPC properties
  - Health, combat, movement, loot, and respawn settings
  - Allows designers to create different NPC types easily

## Core Requirements (G)

### ✅ NPC Spawning & Management
- NPCs spawn randomly at designated spawn points
- Centralized management through NPCManager
- Configurable properties via NPCData ScriptableObject

### ✅ NPC Behavior
- NPCs move around using NavMesh (wander/patrol behavior)
- NPCs deal damage to player when in close proximity
- Damage applied at intervals (cooldown-based, not every frame)

### ✅ NPC Combat & Destruction
- NPCs can receive damage from player weapons (via IDamageable)
- NPCs destroyed when health reaches zero
- Destroyed NPCs drop money at death location
- Destroyed NPCs respawn at random location after delay

### ✅ Vehicle Interaction
- NPCs can be damaged and destroyed by player vehicles
- NPCs do NOT damage vehicles

## Advanced Requirements (VG)

### ✅ Quest System Integration
- NPCs track destruction for quest objectives
- System fires events when NPCs are destroyed:
  - `npcDestroyedEvent` (EmptyPayloadEvent)
  - `npcDestroyedGameObjectEvent` (GameObjectPayloadEvent)
- Quest system can subscribe to these events to track "destroy X NPCs" objectives

### ✅ Save/Load Integration
- NPCManager implements `ISaveable` interface
- Saves active NPC count, spawn cooldowns, and configuration
- Loads state from file (not PlayerPrefs)
- Uses `NPCManagerSaveData` for serialization

## Integration with Existing Systems

### Event System
The NPC system uses the existing event system from `Assets/Tools/EventSystem`:
- Events are ScriptableObjects with payload support
- Events fired when NPCs are destroyed for quest tracking
- Health change events for UI updates

### ServiceLocator Pattern
- NPCManager registers itself as `INPCSpawner` service
- Other systems can access NPCManager without direct dependencies
- Follows the same pattern as other managers in the project

### PlayerReference
- Uses `PlayerReference` for decoupled player access
- No direct dependencies on player GameObject

### IDamageable Interface
- NPCHealth implements `IDamageable` for consistent damage handling
- Allows weapons and other systems to damage NPCs uniformly

## File Structure

```
Assets/Scripts/NPCs/
├── Managers/
│   ├── NPCManager.cs              # Central manager (spawning, tracking, respawning)
│   └── NPCManagerSaveData.cs      # Save data structure
├── Behavior/
│   ├── NPCController.cs           # Main coordinator
│   ├── NPCHealth.cs               # Health component (IDamageable)
│   ├── NPCCombat.cs               # Combat component (damage to player)
│   ├── NPCMovement.cs             # Movement component (NavMesh wander)
│   ├── NPCLoot.cs                 # Loot component (money drops)
│   └── NPCVehicleCollision.cs     # Vehicle collision handling
├── Data/
│   └── NPCData.cs                 # ScriptableObject for NPC configuration
└── AI/
    └── NPCAI.cs                   # Old AI script (can be removed/refactored)
```

## Setup Instructions

### 1. Create NPCData Asset
1. Right-click in Project window
2. Create → Smol Theft Auto → NPC Data
3. Configure health, combat, movement, and loot settings

### 2. Setup NPC Prefab
1. Create/update NPC prefab with these components:
   - NPCController (required)
   - NPCHealth (required)
   - NPCCombat (optional, for player damage)
   - NPCMovement (optional, for AI movement)
   - NPCLoot (optional, for money drops)
   - NPCVehicleCollision (optional, for vehicle damage)
   - Rigidbody (for vehicle collisions)
   - NavMeshAgent (for movement)
2. Assign NPCData ScriptableObject to NPCController

### 3. Setup NPCManager
1. Add NPCManager component to a GameObject in scene
2. Assign NPC prefab
3. Configure spawn settings (initial count, max count, spawn radius, etc.)
4. Assign event channels (npcDestroyedEvent, etc.)

### 4. Setup Events (Quest Integration)
1. Create or use existing events in `Assets/Tools/EventSystem/Events/NPCEvents/`:
   - NPCDestroyedEvent (EmptyPayloadEvent)
   - NPCDestroyedGameObjectEvent (GameObjectPayloadEvent)
2. Assign to NPCHealth components
3. Quest system can subscribe to these events

## Usage Examples

### Dealing Damage to NPC
```csharp
var npcHealth = npc.GetComponent<NPCHealth>();
if (npcHealth != null)
{
    npcHealth.TakeDamage(50f); // Deal 50 damage
}
```

### Accessing NPCManager
```csharp
var spawner = ServiceLocator.Get<INPCSpawner>();
if (spawner != null)
{
    spawner.RespawnNPC(npc, 5f); // Respawn after 5 seconds
}
```

### Quest System Integration
```csharp
// In quest system setup
npcDestroyedGameObjectEvent.OnEventTriggered += OnNPCDestroyed;

private void OnNPCDestroyed(GameObject npc)
{
    // Track NPC destruction for quest objective
    questObjective.Progress++;
}
```

### Save/Load
```csharp
// Save
var npcManager = FindObjectOfType<NPCManager>();
var saveData = npcManager.SaveState();

// Load
npcManager.LoadState(saveData);
```

## Notes

- The old `NPCSpawner` class still exists but is superseded by `NPCManager`
- Old `NPCAI` can be replaced by `NPCMovement` component
- All components are optional except NPCController and NPCHealth
- NPCData is optional but recommended for configuration management
- Vehicle collision detection uses layer mask or "Vehicle" tag

## Design Patterns Used

1. **Component-Based Design**: Each component has single responsibility
2. **ServiceLocator Pattern**: Decoupled service registration and access
3. **Event-Driven Architecture**: ScriptableObject events for communication
4. **Interface Segregation**: IDamageable, ISaveable, INPCSpawner
5. **ScriptableObject Configuration**: NPCData for designer-friendly configuration

## Future Enhancements

- Different NPC types with different behaviors
- NPC state machines (can use existing StateMachine tool)
- NPC groups/squads
- Advanced AI behaviors (chase, flee, patrol routes)

