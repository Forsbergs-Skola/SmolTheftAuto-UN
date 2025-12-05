# Project Contributions — Ibad Ullah

## Overview

My work on **SmolTheftAuto-UN** focused on everything related to **vehicles**:  
how they move, how the player gets in and out of them, and how they interact with
other entities The goal was to build a reusable
vehicle layer that could later be driven by either the player or and AI controller

---

## 1. Vehicle Movement & Physics


I implemented **VehicleMover** a rigidbody based controller that handles the core
physics for all cars in the game.


- Single public API: so any controller (player or AI) can drive the car
- Forward/reverse acceleration using the Rigidbody’s velocity
- Speed clamping so cars never exceed `maxSpeed`
- Steering that only applies when the car is actually moving, to avoid
  weird rotations while stationary
- Simple braking behaviour that quickly slows the car down when `isBraking`
  is true

This script is the **movement brain** for all drivable vehicles.

---

## 2. Player–Vehicle Driving Controller



This script lives on the player and connects the existing player movement system
to the vehicles.



- Handles the **enter / exit vehicle** flow.
- Uses the Unity Input System (`PlayerControls`) to:
  - Detect interaction input (enter/exit)
- When entering a car:
  - Finds the closest valid seat within an `interactionRadius`.
  - Disables the `CharacterController` / `PlayerController` 
  - Snaps the player to the seat’s transform and parents them so they move
    with the car.
- While driving:
  - Converts player input into `SetInput(throttle, steer, brake)` calls on the
    current `VehicleMover`.
- When exiting:
  - Unparents the player from the vehicle.
  - Places the player next to the car (offset from the seat).
  - Re-enables the normal character movement components

---

## 3. Vehicle Seat Interaction Setup



`VehicleSeatInteraction` is a small helper component that marks a collider as a
usable vehicle seat.

This helper keeps the scene setup simple: designers can drop a vehicle, add a
seat trigger, and the player controller will know exactly **where to sit**

---

## 4. Vehicle Damage & Collision

- Cars have their own **health** value and can be destroyed separately from the
  player.
- If the **player is inside a car** and the car’s HP reaches zero:
  - We trigger the appropriate “player died” / failure flow.
- **NPC collisions**:
  - Cars can damage/destroy NPCs when hit at speed.
  - Cars themselves no longer take damage from NPC collisions to prevent
    unfair car deaths from enemy contact.
- **Weapon damage integration**:
  - Hooked vehicles into the existing weapon/bullet logic so cars can now take
    damage from being shot, just like other targets.
  - Ensured that vehicle damage events can later be surfaced to UI (e.g.
    vehicle HP) via the existing event systems.

## 5. Vehicle Scripts Created
*   **AIVehicleController.cs**
*   **PlayerVehicleeController.cs**
*   **VehicleMover.cs**
*   **VehicleSeatInteraction.cs**

## 6. Key Bug Fixes & Integrations

### NPCHealth.cs
*   **Issue:** NPCs were staying alive after being killed.
*   **Fix:** Modified the script to ensure the NPC is properly disabled upon death by turning off the `NavMeshAgent`, `AIController`, and `Collider`.

### VehicleHealth.cs
*   **Issue 1:** Initially targeted only NPC colliders to run them over. When the "Deal Damage to Player" feature was added to NPCs, it broke the code because the NPC's collider was triggering unexpectedly.
*   **Fix 1:** Fixed this by targeting trigger colliders as well.
*   **Issue 2:** When the `AggressiveNPC` variant was added (which includes a Detection System with a large sphere), a bug occurred where the vehicle would take damage from the detection sphere while the player was inside.
*   **Fix 2:** Added a check in the `OnTriggerEnter` method to ignore the detection sphere, resolving the issue.

### System Integrations
*   **Bullet Hit System:** Leveraged the existing Bullet Hit method to implement damage dealing to the vehicle.
*   **Cross-System Connection:** Successfully connected and integrated the NPC systems with the Vehicle and PlayerController systems to ensure seamless interaction.



