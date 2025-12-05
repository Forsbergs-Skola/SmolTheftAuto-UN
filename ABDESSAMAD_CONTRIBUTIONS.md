# ABDESSAMAD CONTRIBUTIONS

**Role:** Advanced NPC System architect for SmolTheftAuto-UN (school group project)

## Key Contributions

### 1. **NPC Death & Respawn Flow** (NPCHealth.cs)
- Immediate cleanup on NPC death: disable colliders, stop NavMeshAgent, disable AI scripts
- Prevents dead NPCs from floating or interfering with gameplay (marked as "ibads Addition")
- Integrated respawn system with NPCSpawner

### 2. **Aggressive NPC AI System** (AggressiveNPC.cs, PlayerDetectionZone.cs)
- Full state machine: Idle → Patrol → Detect → Chase → Attack
- **Group alerting:** All nearby aggressive NPCs alert together when player is detected
- Detection zone with configurable alert propagation radius
- Damage multiplier system for aggressive behavior

### 3. **Patrol NPC System** (PatrolNPC.cs, CheckpointPath.cs)
- Waypoint-based patrol with pause/resume at checkpoints
- Flexible path looping (can end or repeat)

## Files I Created/Modified

**Advanced NPC Scripts:**
- `Assets/Scripts/NPCs/Advanced/AI/` (AggressiveNPC, PatrolNPC, CheckpointPath, PlayerDetectionZone)

**Existing Modifications:**
- NPCHealth.cs: Death cleanup improvements
- MoneyPickup.cs: Trigger-based collection refactor

**Documentation:**
- PROJECT_STRUCTURE.md (updated with NPC layout)

## How to Use

See **README.md** → Advanced NPC System section for full setup guide.

For rapid testing:
1. Assign NPC prefab in NPCSpawner
2. Create CheckpointPath with waypoints for PatrolNPC
3. Set `isAggressive = true` on AggressiveNPC for aggressive behavior
4. Bake NavMesh in scene
5. Play and test

## Testing Notes

- Aggressive NPCs detect player and chase when `isAggressive = true`
- All nearby aggressive NPCs alert together (group mechanic)
- Ragdoll activates on death if RagdollSetup is configured
- Patrol NPCs idle at waypoints for configurable pause time
