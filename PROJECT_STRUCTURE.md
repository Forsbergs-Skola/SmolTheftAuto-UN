# SmolTheftAuto-UN Project Structure

## Overview
This document outlines the organized folder structure for the SmolTheftAuto-UN Unity project. The structure is designed to support team collaboration and maintain clean, organized code.

## Project Organization

### Assets/Scripts/
Main folder for all C# scripts, organized by feature/system.

#### `/Player`
Player-related scripts
- `/Movement` - Player movement and controls
- `/Health` - Player health system
- `/Combat` - Player combat mechanics

#### `/NPCs`
NPC (Non-Player Characters) system - **Abdessamad's Focus Area**
- `/AI` - NPC AI behavior (wandering, patrolling, etc.)
- `/Spawning` - NPC spawning and respawning logic
- `/Behavior` - Core NPC behavior (health, damage, interactions)

#### `/Vehicles`
Vehicle system scripts
- Car entry/exit
- Vehicle driving mechanics
- Vehicle damage system

#### `/Weapons`
Weapon system scripts
- `/Core` - Base weapon classes
- `/Ammo` - Ammunition management

#### `/UI`
User interface scripts
- `/HUD` - Heads-up display (health, money, ammo)
- `/Menus` - Menu systems (pause, main menu, etc.)

#### `/Systems`
Game systems
- `/Store` - Store/purchase system
- `/Quest` - Quest system
- `/SaveLoad` - Save and load game progress

#### `/Managers`
Game managers and singletons
- GameManager, PlayerMoney, etc.

#### `/Data`
Data classes and scriptable objects
- MoneyPickup, ItemData, etc.

### Assets/Prefabs/
Organized prefab folders
- `/Player` - Player prefabs
- `/NPCs` - NPC prefabs
- `/Vehicles` - Vehicle prefabs
- `/Weapons` - Weapon prefabs
- `/UI` - UI prefabs
- `/Environment` - Environment prefabs (buildings, roads, etc.)
- `/Pickups` - Pickup prefabs (money, ammo, etc.)

### Assets/
Other asset folders
- `/KenneyAssets` - Kenney asset packs (already organized)
- `/Materials` - Material files
- `/Models` - 3D models
- `/Audio` - Audio files
- `/Scenes` - Scene files
- `/UI` - UI sprites and textures

## Naming Conventions

### Scripts
- Use PascalCase: `NPCController.cs`, `PlayerHealth.cs`
- Use descriptive names that indicate purpose
- Use namespaces: `SmolTheftAuto.NPCs.Behavior`, `SmolTheftAuto.Player.Health`

### Folders
- Use PascalCase: `NPCs`, `Player`, `Weapons`
- Keep folder names singular or plural consistently

### Prefabs
- Use PascalCase: `NPC_Basic.prefab`, `Player.prefab`
- Prefix with type if needed: `NPC_`, `Vehicle_`, `Weapon_`

## Team Workflow

### Branch Strategy
1. Create feature branches for your work: ` ex : git checkout -b feature/npc-system`
2. Work on your assigned features in your branch
3. Commit frequently with clear messages
4. Push to GitHub and create pull requests for review

### Code Organization
- Keep related scripts together in appropriate folders
- Use namespaces to avoid naming conflicts
- Avoid massive Update() blocks - use events and coroutines
- Create helper/generic functions to avoid repetition
- Comment your code, especially complex logic

### Integration Points
- Use events for communication between systems
- Avoid direct dependencies when possible
- Use managers/singletons for shared state
- Test integration with other systems before merging


