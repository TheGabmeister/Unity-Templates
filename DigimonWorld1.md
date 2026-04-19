# Digimon World 1 — Unity Recreation

A gameplay-focused recreation of Digimon World 1 in Unity. Not a pixel-perfect remake — the goal is faithful mechanics with placeholder 3D models (no animations), placeholder textures, and placeholder audio.

## Scope & Constraints

- Gameplay mechanics over visual fidelity
- Placeholder 3D models, no animations
- Placeholder textures
- Placeholder music and SFX
- Single-player

## Implementation Order

Ordered so each phase builds on the previous one. Anything later that needs an earlier system can assume it's already there.

---

### Phase 0 — Foundation

Project plumbing. Nothing gameplay-facing yet, but everything else depends on it.

1. **Input System** — keyboard, mouse, gamepad; action maps; rebinding hook
2. **Audio System (bare)** — mixer buses (Master/Music/SFX/UI), one-shot helper, music player stub
3. **UI Framework** — canvas setup, screen manager, transitions, base screen class
4. **Debug Tools (bare)** — in-game console, toggleable overlay, FPS counter
5. **Scene / Zone Loader** — async load/unload, loading screen, scene references

### Phase 1 — The Player in a World

Goal: walk a capsule around a test zone with a camera.

6. **Player Movement** — third-person controller, walk/run, gravity, ground check
7. **Camera** — follow camera, look control, collision avoidance
8. **World & Map (one test zone)** — a single zone with walkable terrain and colliders
9. **Zone Transitions** — trigger volumes swap zones via the scene loader
10. **Interaction System** — raycast prompts, `IInteractable` interface, UI prompt

### Phase 2 — Companions, Conversations, Clocks

Goal: partner follows you; NPCs talk; the world has time.

11. **Partner Digimon Follow** — AI-controlled companion, pathfinding/follow, idle behaviors
12. **Dialogue System** — speaker data, branching trees, choice UI, conditional lines
13. **NPC Entities** — dialogue-ready NPCs with patrol/idle behavior
14. **Time System** — in-game clock, day/night cycle, time-gated hooks
15. **HUD** — partner status bar, clock, zone name, currency readout

### Phase 3 — Digimon as Data

Goal: the partner is a real Digimon with stats, needs, and care.

16. **Digimon Data Model** — species, stages, families, types, elements (ScriptableObjects)
17. **Digimon Instance** — runtime stats, age, hidden stats (hunger, weight, tiredness, discipline, happiness, care mistakes)
18. **Care System** — feeding, sleeping, bathroom, praise/scold, training stubs
19. **Item & Inventory System** — item catalog, stackable inventory, use-on-partner, Bits currency
20. **Status / Digimon Info UI** — stat screen, inventory screen
21. **Training Facilities** — mini-game stubs that feed stats, tiredness/happiness gating

### Phase 4 — Combat

Goal: fight wild Digimon, win, lose, flee.

22. **Techniques / Moves Data** — move list, MP cost, element, power
23. **Battle System Core** — encounter start, turn/command flow, damage calc, type/element advantage
24. **Enemy AI (Battle)** — move selection, targeting, retreat thresholds
25. **Status Effects** — poison, paralysis, sleep, confusion, etc.
26. **Battle UI** — HP/MP, commands, tech list, log
27. **Brains-Driven Control** — obedience scaling with Brains/Discipline
28. **Overworld Encounters** — patrol/chase/flee enemies, encounter trigger

### Phase 5 — Progression & Story

Goal: evolution, quests, recruitment, city growth.

29. **Evolution System** — requirements, transition event, devolution, Digitama inheritance
30. **Quest System** — states, objectives, rewards, quest log UI
31. **Recruitment / Befriending** — post-battle recruit logic, conditions
32. **City-Building** — File City expansion tied to recruits, service unlocks (shop/clinic/farm/gym)
33. **Shop UI** — buy/sell against inventory and Bits
34. **Main Story Quests + Side Quests (content pass)**

### Phase 6 — Presentation & Persistence

Goal: scripted moments, permanence, polish.

35. **Cutscene System** — timeline-style authoring, skippable, triggered cutscenes
36. **Save / Load System** — multi-slot, autosave, full serialization of all the above, versioning
37. **Main Menu & Save Slot UI** — new game, continue, load, options
38. **Settings Menu** — audio mix, controls rebind, graphics, text speed, subtitles
39. **Full Audio Pass** — zone music, battle music, SFX coverage, crossfades
40. **Debug / Cheat Menu (full)** — spawn, evolve, teleport, fast-forward time, save inspector

---

## Systems Reference

Full list grouped by domain — same systems as above, for quick scanning.

### Digimon & Progression
- Digimon data & instances
- Stats & hidden stats
- Life stages & lifespan
- Evolution & devolution
- Digitama inheritance

### Gameplay Loops
- Care (feed/sleep/bathroom/praise/scold)
- Training
- Battle + techniques + status effects
- Recruitment / befriending
- Quests
- City-building

### World
- Player movement & camera
- Partner follow
- Zones & transitions
- Overworld encounters
- Time / day-night

### Characters
- NPCs
- Enemy AI (overworld + battle)
- Partner AI

### Narrative
- Dialogue system
- Cutscenes

### Meta / Framework
- Input
- UI & menus (HUD, status, inventory, quest log, battle, shop, save/load, settings)
- Audio
- Save/load
- Settings & config
- Debug tools

## Content Data (placeholder-backed)

- Digimon roster and evolution tree
- Techniques / move list
- Item catalog
- NPC roster
- Quest definitions
- Zone / map definitions
- Dialogue scripts
- Cutscene scripts

## Out of Scope (for now)

- Animations
- Final art and models
- Final music and SFX
- Multiplayer
- Localization content (hook only)
