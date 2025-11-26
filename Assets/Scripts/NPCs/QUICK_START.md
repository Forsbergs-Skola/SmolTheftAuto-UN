# NPC System Quick Start Guide

> 📋 **See `INDEX.md` for complete file listing of all NPC system files**

# NPC System Quick Start Guide

## Quick Setup (5 minutes)

### Step 1: Create NPCData Asset
1. Right-click in Project window → Create → Smol Theft Auto → NPC Data
2. Name it "NPC_Basic_Data"
3. Configure values (or leave defaults)

### Step 2: Setup NPC Prefab
Your NPC prefab should have these components:
- ✅ **NPCController** (assign NPCData asset here)
- ✅ **NPCHealth** (health management)
- ✅ **NPCCombat** (damage to player) - Optional
- ✅ **NPCMovement** (wander behavior) - Optional
- ✅ **NPCLoot** (money drops) - Optional  
- ✅ **NPCVehicleCollision** (vehicle damage) - Optional
- ✅ **Rigidbody** (for collisions)
- ✅ **NavMeshAgent** (for movement)

### Step 3: Setup NPCManager in Scene
1. Create empty GameObject named "NPCManager"
2. Add **NPCManager** component
3. Assign your NPC prefab
4. Configure spawn settings:
   - Initial NPC Count: 10
   - Max NPC Count: 20
   - Spawn Radius: 50
5. Assign event assets from `Assets/Tools/EventSystem/Events/NPCEvents/`

### Step 4: Setup Events for Quest System
1. Find/create these event assets:
   - `NPCDestroyedEvent` (EmptyPayloadEvent)
   - `NPCDestroyedGameObjectEvent` (GameObjectPayloadEvent)
2. Assign to NPCHealth component's event channels
3. Quest system can subscribe to these events

## Component Checklist

### Required Components
- [ ] NPCController
- [ ] NPCHealth

### Optional Components (Add as needed)
- [ ] NPCCombat - NPCs damage player
- [ ] NPCMovement - NPCs wander around
- [ ] NPCLoot - NPCs drop money on death
- [ ] NPCVehicleCollision - NPCs can be hit by vehicles

## Common Issues

**Q: NPCs don't spawn**
- Check NPCManager has prefab assigned
- Check spawn radius isn't too small
- Check layer mask includes ground

**Q: NPCs don't move**
- Add NavMeshAgent component
- Add NPCMovement component
- Check NavMesh is baked in scene

**Q: NPCs don't drop money**
- Add NPCLoot component
- Assign money pickup prefab
- Check money pickup prefab exists

**Q: Quest system not tracking NPCs**
- Assign event assets to NPCHealth
- Subscribe to events in quest system
- Check events are firing (use Debug.Log)

## Migration from Old System

If you have the old `NPCSpawner`:
1. Replace with `NPCManager` (they're compatible - same interface)
2. Old `NPCAI` can be replaced with `NPCMovement`
3. Old `NPCController` logic is now split into components

## Testing

1. Enter Play mode
2. NPCs should spawn automatically
3. Shoot NPCs - they should take damage and die
4. NPCs should drop money when destroyed
5. NPCs should respawn after delay
6. Run NPCs over with vehicle - they should take damage

## Next Steps

- Create different NPC types with different NPCData assets
- Add more complex behaviors to NPCMovement
- Customize loot drops per NPC type
- Add NPC animations/states

