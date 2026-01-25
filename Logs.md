# Gameplay Logs

## Rule System
- [RuleSystem] Not enough energy to play unit
- [RuleSystem] Player army cap reached
- [RuleSystem] Card allowed

## Match Engine
- [Gate] Card requested
- [Clerk] Executing card spawn

## Game Manager
- TIME OVER → Resolving match
- Match Ended → Result scene loaded

## Purpose
Logs provide clear visibility into why actions succeeded or failed.

## Spawn & Projectile Guards

- Implemented enemy unit cap to prevent unlimited spawns
- Spawn attempts are rejected when unit cap is reached
- Projectile firing is blocked when projectile cap is exceeded
- All rejected spawns and projectiles log clear reasons in console
- System fails gracefully without crashes or performance spikes

Status: Guarded build ready

