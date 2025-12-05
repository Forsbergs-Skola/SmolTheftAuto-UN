# SmolTheftAuto-UN

A Christmas-themed Small Theft Auto game built in Unity with C#.

## Project Overview

- **Team Size:** 5 members
- **Duration:** 3 weeks
- **Theme:** Christmas/Holiday
- **Assets:** Kenney Assets (Holiday Kit, Buildings, Roads, Vehicles, Characters)

## Project Structure

See [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) for detailed folder organization.

## Individual Contributions
- Brad: Architecture, backend systems, UI -- [BRAD_CONTRIBUTIONS.md](BRAD_CONTRIBUTIONS.md)
- Abdessamad: NPC System, Project Stracture... [ABDESSAMAD_CONTRIBUTIONS.md](ABDESSAMAD_CONTRIBUTIONS.md)
- Ibad :Vehicle Systems or Vroom Vroom *crashnoise* *runningoverNPCnoise* [Ibads Contributions](https://github.com/Forsbergs-Skola/SmolTheftAuto-UN/blob/main/ibad's%20Contributions.md) 


## Features

### Core Features (Tasks for G)
- ✅ Player movement and controls
- ✅ World with roads and buildings
- ✅ NPC spawning and behavior
- ✅ NPC damage system
- ✅ Player health and regeneration
- ✅ Weapon system (handgun, machine gun)
- ✅ Weapon reloading
- ✅ Money system
- ✅ Vehicle system (entry/exit, driving, damage)

### Additional Features (Tasks for VG)
- ✅ Ammunition system
- ✅ Store system
- ✅ Grenade system
- ✅ Quest system
- ✅ Save/Load system

## Caveats

This section briefly outlines known bugs, missing features, and optimization opportunities that we are aware of, but were not able to address before the final change freeze of the alpha release (5th December 2025) 

### Bugs
- After dying, the game requires a hard restart to refresh the HUD health bar in a new game

### Features & Improvements
- Replace placeholder UI elements with professional assets
- Add respawn logic to all NPC types
- The Car is slippery. we could fix it or add a line to the mechanic saying how "slippery the road is"

### Optimization & Performance
- Backend refactor: replace tag-based singletons with static instances

## Getting Started

### Setup
1. Clone the repository
2. Open the project in Unity
3. See feature-specific guides in respective folders

## Team Workflow

1. Create feature branches for your work
2. Work on assigned features
3. Commit frequently with clear messages
4. Push to GitHub and create pull requests
5. Review and merge after approval

## Code Standards

- Use namespaces: `SmolTheftAuto.FeatureName`
- Avoid massive Update() blocks
- Use events for system communication
- Create helper functions to avoid repetition
- Comment complex logic
- Follow folder structure guidelines

## Resources

- [Unity Documentation](https://docs.unity3d.com/)
- [Kenney Assets](https://kenney.nl/)
- [Project Structure Guide](PROJECT_STRUCTURE.md)

## License

School Project - Educational Use
