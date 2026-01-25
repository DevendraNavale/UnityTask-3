# UnityTask-3
submission of task 3
# VR Strategy Game (Unity)

## Project Overview
A VR-based real-time strategy game built in Unity.
Players deploy units using cards while managing energy and army limits.

## Features Implemented
- Card-based unit deployment
- Energy validation & army caps (Rule System)
- Enemy AI with controlled spawning
- Match flow management (start, end, result)
- Restart-safe scene flow
- Debug logging for gameplay decisions

## Controls
- Select cards from UI to spawn units
- Match ends on timer or base destruction

## Development Notes
- Designed primarily for VR
- Tested partially in non-VR using editor play mode
- Full gameplay tested with VR headset

## Folder Structure
- Assets/ → Core gameplay scripts & prefabs
- Managers/ → Match, rule, flow systems
- Gameplay/ → Player & enemy systems

## Stability & Guards
- Unit cap enforced
- Projectile cap enforced
- Spawn rejection logged
- Restart safe (no duplicates)

## Author
Devendra
