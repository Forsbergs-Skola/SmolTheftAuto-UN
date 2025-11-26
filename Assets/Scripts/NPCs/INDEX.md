# NPC System - Complete File Index

This document lists all files in the NPC system so your team can easily find everything.

## 📁 Folder Structure

All NPC system files are located in: `Assets/Scripts/NPCs/`

```
Assets/Scripts/NPCs/
├── 📋 INDEX.md                    ← YOU ARE HERE
├── 📖 README_NPC_SYSTEM.md        ← Full documentation
├── 🚀 QUICK_START.md              ← Quick setup guide
├── 🎮 SCENE_SETUP_GUIDE.md        ← Complete scene setup with Kenney Assets ⭐
│
├── 📂 Managers/
│   └── NPCManager.cs              ← Central manager (spawning, tracking, respawning)
│
├── 📂 Behavior/
│   ├── NPCController.cs           ← Main coordinator for all components
│   ├── NPCHealth.cs               ← Health management (implements IDamageable)
│   ├── NPCCombat.cs               ← Deals damage to player
│   ├── NPCMovement.cs             ← Movement/wander behavior (NavMesh)
│   ├── NPCLoot.cs                 ← Drops money on death
│   └── NPCVehicleCollision.cs     ← Vehicle collision handling
│
├── 📂 Data/
│   ├── NPCData.cs                 ← ScriptableObject for NPC configuration
│   └── NPCManagerSaveData.cs      ← Save/load data structure
│
├── 📂 Spawning/ (Legacy)
│   └── NPCSpawner.cs              ← Old spawner (superseded by NPCManager)
│
└── 📂 AI/ (Legacy)
    └── NPCAI.cs                   ← Old AI (superseded by NPCMovement)
```

## 📝 File Descriptions

### Core Manager
| File | Description | Used By |
|------|-------------|---------|
| `Managers/NPCManager.cs` | Central manager for NPC spawning, tracking, and respawning. Implements `INPCSpawner` and `ISaveable`. | Scene GameObject |

### Behavior Components
| File | Description | Required? |
|------|-------------|-----------|
| `Behavior/NPCController.cs` | Main coordinator that references all components | ✅ Required |
| `Behavior/NPCHealth.cs` | Health management, implements `IDamageable` | ✅ Required |
| `Behavior/NPCCombat.cs` | Deals damage to player at intervals | ⚪ Optional |
| `Behavior/NPCMovement.cs` | Handles patrol/wander behavior using NavMesh | ⚪ Optional |
| `Behavior/NPCLoot.cs` | Drops money when NPC dies | ⚪ Optional |
| `Behavior/NPCVehicleCollision.cs` | Handles vehicle collision damage | ⚪ Optional |

### Data Classes
| File | Description | Type |
|------|-------------|------|
| `Data/NPCData.cs` | ScriptableObject for configurable NPC properties | Create Asset |
| `Data/NPCManagerSaveData.cs` | Save data structure for NPCManager state | Data Class |

### Legacy Files (Can be removed)
| File | Status | Replacement |
|------|--------|-------------|
| `Spawning/NPCSpawner.cs` | Legacy | Use `NPCManager` instead |
| `AI/NPCAI.cs` | Legacy | Use `NPCMovement` instead |

## 🔗 Related Files Outside NPCs Folder

These files are used by the NPC system but are part of the shared Core/Tools:

| File | Location | Purpose |
|------|----------|---------|
| `IDamageable.cs` | `Assets/Scripts/Core/` | Interface for damageable entities |
| `ISaveable.cs` | `Assets/Scripts/Core/` | Interface for save/load functionality |
| `INPCSpawner.cs` | `Assets/Scripts/Core/` | Interface for NPC spawning |
| `ServiceLocator.cs` | `Assets/Scripts/Core/` | Service registration pattern |
| `PlayerReference.cs` | `Assets/Scripts/Core/` | Player access without dependencies |
| Event Assets | `Assets/Tools/EventSystem/Events/NPCEvents/` | Quest integration events |

## 🎯 Quick Navigation

### To Add NPC Functionality:
1. **Health System** → `Behavior/NPCHealth.cs`
2. **Combat System** → `Behavior/NPCCombat.cs`
3. **Movement System** → `Behavior/NPCMovement.cs`
4. **Loot System** → `Behavior/NPCLoot.cs`
5. **Vehicle Collisions** → `Behavior/NPCVehicleCollision.cs`

### To Configure NPCs:
1. **Create NPC Data** → `Data/NPCData.cs` (Create Asset)
2. **Setup Manager** → `Managers/NPCManager.cs`

### To Understand the System:
1. **Quick Start** → `QUICK_START.md`
2. **Full Documentation** → `README_NPC_SYSTEM.md`
3. **This Index** → `INDEX.md`

## 📊 Component Dependency Graph

```
NPCManager
    └── Spawns → NPC Prefab
            └── NPCController (coordinates all)
                    ├── NPCHealth (required)
                    ├── NPCCombat (optional)
                    ├── NPCMovement (optional)
                    ├── NPCLoot (optional)
                    └── NPCVehicleCollision (optional)
```

## ✅ Checklist for Team Members

When working on NPC system:

- [ ] All new NPC code goes in `Assets/Scripts/NPCs/`
- [ ] Use existing components when possible
- [ ] Follow component-based design (one responsibility per component)
- [ ] Use events for cross-system communication
- [ ] Update this index if adding new files
- [ ] Check `README_NPC_SYSTEM.md` for design patterns

## 🆕 Recent Changes

- ✅ Created NPCManager (replaces NPCSpawner)
- ✅ Refactored to component-based architecture
- ✅ Added vehicle collision support
- ✅ Integrated save/load functionality
- ✅ Added quest system integration (via events)

---

**Last Updated:** After NPC System Refactor
**Maintainer:** NPC System Team

