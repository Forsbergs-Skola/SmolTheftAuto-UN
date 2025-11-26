# Complete Scene Setup Guide - NPC System Testing

> 🎯 **Goal**: Create a complete test scene for the NPC system using Kenney Assets  
> ⏱️ **Time**: 30-45 minutes  
> 📦 **Prerequisites**: Unity project with Kenney Assets and NPC System installed

---

## Table of Contents

1. [Create New Scene](#step-1-create-new-scene)
2. [Setup Environment with Kenney Assets](#step-2-setup-environment-with-kenney-assets)
3. [Setup Ground and NavMesh](#step-3-setup-ground-and-navmesh)
4. [Setup Lighting](#step-4-setup-lighting)
5. [Setup Player](#step-5-setup-player)
6. [Create NPC Data Asset](#step-6-create-npc-data-asset)
7. [Setup NPC Prefab](#step-7-setup-npc-prefab)
8. [Setup NPCManager](#step-8-setup-npcmanager)
9. [Setup Core Systems](#step-9-setup-core-systems)
10. [Setup Events](#step-10-setup-events)
11. [Final Checks and Testing](#step-11-final-checks-and-testing)

---

## Step 1: Create New Scene

### 1.1 Create Scene File

1. In Unity, go to **File → New Scene**
2. Choose **Basic (Built-in)** or your project's template
3. Click **Create**
4. Save the scene:
   - **File → Save As**
   - Navigate to `Assets/Scenes/`
   - Name it: `NPC_Test_Scene.unity`
   - Click **Save**

### 1.2 Clean Up Default Objects

1. Delete the default **Main Camera** (we'll create a better one)
2. Keep the **Directional Light** (or delete if you prefer custom lighting)

---

## Step 2: Setup Environment with Kenney Assets

### 2.1 Find Kenney Assets

Kenney Assets are located in: `Assets/KenneyAssets/`

Available asset packs:
- **K-Buildings** - Buildings for the city
- **K-CityRoads** - Roads and street elements
- **K-CharactersMini** - Character models (for NPCs)
- **K-Vehicles** - Cars and vehicles
- **K-NatureKit** - Trees, grass, nature elements
- **K-HolidayKit** - Holiday/Christmas themed items

### 2.2 Create Ground Plane

1. Right-click in Hierarchy → **3D Object → Plane**
2. Rename it to: `Ground`
3. Set Transform:
   - Position: `(0, 0, 0)`
   - Rotation: `(0, 0, 0)`
   - Scale: `(10, 1, 10)` (makes it 100x100 units)

### 2.3 Add Ground Material

1. In Project window, go to `Assets/Materials/`
2. If `Ground.mat` exists, drag it onto the Ground plane
3. If not, create a new material:
   - Right-click in `Assets/Materials/` → **Create → Material**
   - Name it: `Ground`
   - Set Albedo color to gray/brown
   - Drag onto Ground plane

### 2.4 Add Some Buildings

1. In Project window, navigate to `Assets/KenneyAssets/K-Buildings/`
2. Find building models (e.g., `building_01.fbx`, `building_02.fbx`)
3. Drag 3-5 buildings into the scene
4. Position them around the ground:
   - Scatter them randomly or create a small city layout
   - Leave space for NPCs to move between buildings
   - Set Y position so buildings sit on the ground

**Tip**: Create an empty GameObject called `Buildings` and parent all buildings to it for organization.

### 2.5 Add Roads (Optional but Recommended)

1. Navigate to `Assets/KenneyAssets/K-CityRoads/`
2. Find road pieces (e.g., `road_straight.fbx`, `road_corner.fbx`)
3. Drag road pieces to create paths between buildings
4. Make sure roads are at ground level (Y = 0 or slightly above)

### 2.6 Add Nature Elements (Optional)

1. Navigate to `Assets/KenneyAssets/K-NatureKit/`
2. Add some trees/grass around the scene for atmosphere
3. Don't place too many - we need space for NPCs

### 2.7 Organize Hierarchy

Create empty GameObjects to organize the scene:

```
Scene Hierarchy:
├── Environment
│   ├── Ground
│   ├── Buildings
│   ├── Roads
│   └── Nature
├── Player (we'll add this later)
└── NPCSystem (we'll add this later)
```

---

## Step 3: Setup Ground and NavMesh

### 3.1 Mark Ground as Navigation Static

1. Select the **Ground** plane
2. In Inspector, find the **Static** dropdown (top right)
3. Check **Navigation Static** (you may need to click the dropdown to see options)
4. If prompted, click **Yes, change children** (if ground has children)

### 3.2 Mark Buildings as Navigation Static

1. Select all buildings
2. Set them as **Navigation Static**
3. This tells NavMesh that NPCs should walk around buildings, not through them

**Note**: You can also mark buildings as **Navigation Static** but set them to **Not Walkable** in NavMesh settings if you want NPCs to avoid them entirely.

### 3.3 Bake NavMesh

1. Go to **Window → AI → Navigation** (opens NavMesh window)
2. Click the **Bake** tab
3. Adjust settings if needed:
   - **Agent Radius**: 0.5
   - **Agent Height**: 2
   - **Max Slope**: 45
   - **Step Height**: 0.4
4. Click **Bake** button (bottom right of window)
5. Wait for baking to complete
6. You should see a blue overlay on the ground - this is the NavMesh!

**Troubleshooting**:
- If you don't see blue overlay, make sure Ground has **Navigation Static** checked
- If NavMesh is too small, check that Ground scale is large enough
- Make sure buildings are marked as Navigation Static

---

## Step 4: Setup Lighting

### 4.1 Setup Basic Lighting

1. Create a **Directional Light** if you don't have one:
   - Right-click Hierarchy → **Light → Directional Light**
2. Position it above the scene:
   - Position: `(0, 10, 0)`
   - Rotation: `(50, -30, 0)` (nice angled lighting)

### 4.2 Setup Camera

1. Create a **Main Camera**:
   - Right-click Hierarchy → **Camera**
   - Rename to: `Main Camera`
2. Position it to look at the scene:
   - Position: `(0, 10, -15)`
   - Rotation: `(20, 0, 0)`
3. Adjust camera settings:
   - **Field of View**: 60-75 (adjust to preference)
   - **Background**: Set to a sky blue color

**Tip**: Use **Scene View** to position camera, then press **Ctrl+Shift+F** (or Cmd+Shift+F on Mac) to match Scene view camera to Game view.

---

## Step 5: Setup Player

### 5.1 Find or Create Player Prefab

Check if player prefab exists:
- Look in `Assets/Prefabs/Player/`

**Option A: Use Existing Player Prefab**

1. Find player prefab (e.g., `Player.prefab`)
2. Drag it into the scene
3. Position at: `(0, 1, 0)` (1 unit above ground)

**Option B: Create Simple Player (If no prefab exists)**

1. Create a **Capsule**:
   - Right-click Hierarchy → **3D Object → Capsule**
   - Rename to: `Player`
2. Position at: `(0, 1, 0)`
3. Add **Rigidbody** component:
   - Add Component → **Rigidbody**
   - Set **Freeze Rotation** on X and Z (only allow Y rotation)
4. Add Player script (if you have one):
   - Add Component → Find your player movement script
5. Set Tag:
   - In Inspector, set **Tag** to: `Player`

### 5.2 Setup PlayerReference

1. Create empty GameObject: `PlayerReference`
2. Add Component → Search for: `PlayerReference`
3. This allows NPCs to find the player without direct dependencies

### 5.3 Verify Player Setup

- Player should have **Tag** set to `Player`
- Player should be positioned above ground
- Player should have movement controls (if player script exists)

---

## Step 6: Create NPC Data Asset

### 6.1 Create NPCData ScriptableObject

1. In Project window, navigate to `Assets/Data/` or create folder `Assets/Data/NPCs/`
2. Right-click → **Create → Smol Theft Auto → NPC Data**
3. Name it: `NPC_Basic_Data`
4. Configure settings in Inspector:

**Health Settings:**
- Max Health: `100`

**Combat Settings:**
- Damage To Player: `10`
- Damage Range: `2`
- Damage Cooldown: `1`

**Movement Settings:**
- Movement Speed: `3`
- Wander Radius: `10`
- Wander Timer: `5`

**Loot Settings:**
- Min Money Drop: `10`
- Max Money Drop: `50`

**Respawn Settings:**
- Should Respawn: `true` ✓
- Respawn Delay: `5`

**Save the asset!**

---

## Step 7: Setup NPC Prefab

### 7.1 Find or Update NPC Prefab

**Option A: Update Existing NPC Prefab**

1. Find `Assets/Prefabs/NPCs/NPC_Basic.prefab`
2. Double-click to open in Prefab Mode
3. Follow steps below to add components

**Option B: Create New NPC Prefab**

1. Create a **Capsule** in scene:
   - Right-click Hierarchy → **3D Object → Capsule**
   - Rename to: `NPC_Basic`
   - Position at: `(0, 1, 0)`
2. Follow steps below to add components
3. Drag to `Assets/Prefabs/NPCs/` to create prefab

### 7.2 Add Required Components

Add these components one by one:

#### 7.2.1 Rigidbody
1. **Add Component → Physics → Rigidbody**
2. Settings:
   - **Freeze Rotation**: X ✓, Z ✓ (only allow Y rotation)
   - **Collision Detection**: Continuous (prevents passing through objects)

#### 7.2.2 Capsule Collider (should already exist)
- **Is Trigger**: Unchecked
- **Radius**: 0.5
- **Height**: 2

#### 7.2.3 NavMeshAgent
1. **Add Component → Navigation → Nav Mesh Agent**
2. Settings:
   - **Radius**: 0.5
   - **Height**: 2
   - **Speed**: 3.5
   - **Acceleration**: 8
   - **Stopping Distance**: 0.5

#### 7.2.4 NPCController (Required)
1. **Add Component → Search: NPCController**
2. Assign **NPC Data**: Drag `NPC_Basic_Data` asset here

#### 7.2.5 NPCHealth (Required)
1. **Add Component → Search: NPCHealth**
2. Settings will auto-load from NPCData
3. Event Channels (we'll assign these later in Step 10)

#### 7.2.6 NPCCombat (Optional - for damaging player)
1. **Add Component → Search: NPCCombat**
2. Settings will auto-load from NPCData

#### 7.2.7 NPCMovement (Optional - for wander behavior)
1. **Add Component → Search: NPCMovement**
2. Settings will auto-load from NPCData
3. **Requires NavMeshAgent** (already added above)

#### 7.2.8 NPCLoot (Optional - for money drops)
1. **Add Component → Search: NPCLoot**
2. Assign **Money Pickup Prefab**:
   - Find `Assets/Prefabs/Pickups/MoneyPickup.prefab`
   - Drag into the **Money Pickup Prefab** field

#### 7.2.9 NPCVehicleCollision (Optional - for vehicle damage)
1. **Add Component → Search: NPCVehicleCollision**
2. Set **Vehicle Layer**:
   - Check what layer vehicles are on (usually a "Vehicle" layer)
   - Set **Vehicle Layer** mask to include that layer

### 7.3 Set NPC Layer

1. In Inspector, set **Layer** to: `NPC` (or create one if it doesn't exist)
2. This helps with collision detection

### 7.4 Create Prefab (if creating new)

1. Drag the configured NPC from scene to `Assets/Prefabs/NPCs/`
2. Name it: `NPC_Basic`
3. Delete the instance from scene (we'll spawn them via NPCManager)

### 7.5 Component Checklist

Your NPC prefab should have:
- ✅ Rigidbody
- ✅ Capsule Collider
- ✅ NavMeshAgent
- ✅ NPCController (with NPCData assigned)
- ✅ NPCHealth
- ✅ NPCCombat (optional)
- ✅ NPCMovement (optional)
- ✅ NPCLoot (optional)
- ✅ NPCVehicleCollision (optional)

---

## Step 8: Setup NPCManager

### 8.1 Create NPCManager GameObject

1. Right-click in Hierarchy → **Create Empty**
2. Rename to: `NPCManager`
3. Position at: `(0, 0, 0)` (doesn't matter, but center is nice)

### 8.2 Add NPCManager Component

1. **Add Component → Search: NPCManager**
2. Configure settings:

**Spawn Settings:**
- **NPC Prefab**: Drag `Assets/Prefabs/NPCs/NPC_Basic.prefab` here
- **Initial NPC Count**: `10`
- **Max NPC Count**: `20`
- **Spawn Radius**: `50`
- **Min Spawn Distance From Player**: `10`
- **Spawn Check Layers**: Set to include **Ground** layer

**Spawn Area:**
- **Use Spawn Center**: Unchecked (spawns around player)
- **Spawn Center**: Leave at `(0, 0, 0)` if using above setting

**Event Channels:**
- **NPC Destroyed Event**: (we'll assign in Step 10)

### 8.3 Organize in Hierarchy

Create a folder structure:

```
NPCSystem
└── NPCManager
```

---

## Step 9: Setup Core Systems

### 9.1 Setup PlayerReference (if not done)

1. Find or create `PlayerReference` GameObject
2. Add `PlayerReference` component
3. This should automatically find the Player by tag

### 9.2 Setup GameManager (if needed)

1. Check if GameManager exists: `Assets/Prefabs/GameManager/`
2. If exists, drag into scene
3. If not, you can skip this (NPC system works independently)

### 9.3 Setup Canvas/UI (Optional)

1. If you have UI prefabs, add them:
   - Check `Assets/Prefabs/UI/`
   - Drag UI prefabs into scene if needed

---

## Step 10: Setup Events

### 10.1 Find or Create Event Assets

Events should be in: `Assets/Tools/EventSystem/Events/NPCEvents/`

**Required Events:**
1. `NPCDestroyedEvent` (EmptyPayloadEvent)
2. `NPCDestroyedGameObjectEvent` (GameObjectPayloadEvent)

### 10.2 Create Events (If They Don't Exist)

**Create NPCDestroyedEvent:**
1. Right-click in `Assets/Tools/EventSystem/Events/NPCEvents/`
2. **Create → Event Channels → Empty Payload Event**
3. Name it: `NPCDestroyedEvent`

**Create NPCDestroyedGameObjectEvent:**
1. Right-click in same folder
2. **Create → Event Channels → GameObject Payload Event**
3. Name it: `NPCDestroyedGameObjectEvent`

### 10.3 Assign Events to NPCManager

1. Select **NPCManager** in scene
2. In Inspector, find **NPC Manager** component
3. Drag `NPCDestroyedGameObjectEvent` into **NPC Destroyed Event** field

### 10.4 Assign Events to NPC Prefab

1. Open `NPC_Basic` prefab (double-click)
2. Select the root GameObject
3. Find **NPC Health** component
4. Assign events:
   - Drag `NPCDestroyedEvent` to **NPC Destroyed Event**
   - Drag `NPCDestroyedGameObjectEvent` to **NPC Destroyed Game Object Event**
5. **Save** the prefab (Ctrl+S or File → Save)

**Note**: If using NPCData, events might be inherited. Check NPCHealth component in prefab.

---

## Step 11: Final Checks and Testing

### 11.1 Pre-Flight Checklist

Before pressing Play, verify:

**Scene Setup:**
- [ ] Ground exists and has Navigation Static checked
- [ ] NavMesh is baked (blue overlay visible)
- [ ] Buildings are marked as Navigation Static
- [ ] Lighting is set up
- [ ] Camera positioned correctly

**Player Setup:**
- [ ] Player exists in scene
- [ ] Player has Tag set to `Player`
- [ ] PlayerReference component exists
- [ ] Player can move (test controls)

**NPC System:**
- [ ] NPCData asset created and configured
- [ ] NPC prefab has all required components
- [ ] NPC prefab has NPCData assigned
- [ ] NPCManager exists in scene
- [ ] NPCManager has NPC prefab assigned
- [ ] Events assigned to NPCManager and NPC prefab

### 11.2 Enter Play Mode

1. **Save Scene**: Ctrl+S (Cmd+S on Mac)
2. **Save Project**: Ctrl+S (saves all assets)
3. Click **Play** button ▶️

### 11.3 What to Expect

**On Play:**
- NPCs should spawn around the player (within spawn radius)
- NPCs should start wandering around (if NPCMovement is enabled)
- You should see 10 NPCs (initial spawn count)

### 11.4 Test NPC Functionality

**Test 1: NPC Spawning**
- [ ] NPCs spawn when scene starts
- [ ] NPCs are positioned around player (not too close)
- [ ] Can see 10 NPCs in scene

**Test 2: NPC Movement**
- [ ] NPCs move around (if NPCMovement enabled)
- [ ] NPCs avoid buildings
- [ ] NPCs stay on NavMesh (don't fall through ground)

**Test 3: NPC Combat (if enabled)**
- [ ] Move player close to NPC
- [ ] Player takes damage at intervals (check health)
- [ ] Damage doesn't happen every frame (has cooldown)

**Test 4: NPC Destruction**
- [ ] Shoot/damage NPC (if player has weapon)
- [ ] NPC health decreases
- [ ] NPC is destroyed when health reaches 0
- [ ] Money drops when NPC dies (if NPCLoot enabled)

**Test 5: NPC Respawn**
- [ ] Destroy an NPC
- [ ] Wait 5 seconds (respawn delay)
- [ ] New NPC spawns at random location

**Test 6: Vehicle Collision (if enabled)**
- [ ] Drive vehicle into NPC
- [ ] NPC takes damage
- [ ] NPC can be destroyed by vehicle

### 11.5 Common Issues and Solutions

**Issue: NPCs don't spawn**
- ✅ Check NPCManager has prefab assigned
- ✅ Check spawn radius is large enough
- ✅ Check player exists and is tagged "Player"
- ✅ Check console for errors

**Issue: NPCs don't move**
- ✅ Check NavMeshAgent component exists
- ✅ Check NPCMovement component exists
- ✅ Check NavMesh is baked
- ✅ Check NPCs are on NavMesh (not floating)

**Issue: NPCs fall through ground**
- ✅ Check Ground has collider
- ✅ Check NPC Rigidbody settings
- ✅ Check NPC position is above ground

**Issue: NPCs don't damage player**
- ✅ Check NPCCombat component exists
- ✅ Check player is within damage range
- ✅ Check damage cooldown has passed

**Issue: No money drops**
- ✅ Check NPCLoot component exists
- ✅ Check Money Pickup Prefab is assigned
- ✅ Check MoneyPickup prefab exists

**Issue: Errors in console**
- ✅ Read error messages carefully
- ✅ Check all prefab references are assigned
- ✅ Check all required components exist
- ✅ Check event assets exist

---

## 🎉 Success!

If everything works, you now have a fully functional NPC test scene!

### Next Steps

1. **Customize NPCs**: Create different NPCData assets for different NPC types
2. **Add More NPCs**: Increase spawn count or create spawn zones
3. **Add Weapons**: Test NPC combat with player weapons
4. **Add Vehicles**: Test vehicle collision with NPCs
5. **Add UI**: Show NPC count, health, etc.
6. **Quest Integration**: Connect NPC destruction events to quest system

### Scene File Location

Your scene is saved at: `Assets/Scenes/NPC_Test_Scene.unity`

---

## 📝 Quick Reference

### Important Paths

- **Kenney Assets**: `Assets/KenneyAssets/`
- **NPC Scripts**: `Assets/Scripts/NPCs/`
- **NPC Prefabs**: `Assets/Prefabs/NPCs/`
- **Event Assets**: `Assets/Tools/EventSystem/Events/NPCEvents/`
- **NPC Data**: `Assets/Data/NPCs/` (or wherever you saved it)

### Key Components

- **NPCManager**: Manages spawning and tracking
- **NPCController**: Coordinates all NPC components
- **NPCHealth**: Health management (required)
- **NPCCombat**: Deals damage to player
- **NPCMovement**: Wander behavior
- **NPCLoot**: Drops money on death
- **NPCVehicleCollision**: Vehicle collision handling

### Important Tags/Layers

- **Player Tag**: `Player` (required)
- **NPC Layer**: `NPC` (optional but recommended)
- **Ground Layer**: Mark as Navigation Static

---

**Need Help?** Check:
- `QUICK_START.md` - Quick setup guide
- `README_NPC_SYSTEM.md` - Full documentation
- `INDEX.md` - File listing

Happy testing! 🚀

