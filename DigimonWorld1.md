# Digimon World 1 — Unity Recreation

A gameplay-focused recreation of Digimon World 1 in Unity. Not a pixel-perfect remake — the goal is faithful mechanics with placeholder 3D models (no animations), placeholder textures, and placeholder audio.

## Scope & Constraints

- Gameplay mechanics over visual fidelity
- Placeholder 3D models, no animations
- Placeholder textures
- Placeholder music and SFX
- Single-player

## Core Systems

### Digimon System
- Digimon data (species, stages, families, types, elements)
- Individual Digimon instances (unique stats, age, lifespan)
- Stats: HP, MP, Offense, Defense, Speed, Brains
- Hidden stats: Happiness, Discipline, Weight, Tiredness, Hunger, Care Mistakes
- Life stages: Digi-Egg → Baby → In-Training → Rookie → Champion → Ultimate
- Death and reincarnation back to Digi-Egg
- Personality / mood

### Evolution System
- Stage-based evolution (Rookie → Champion → Ultimate)
- Evolution requirements (stats, care mistakes, weight, training, bonuses)
- Devolution on death
- Digitama (egg) generation with inherited bonuses
- Evolution reveal/transition event

### Battle System
- Real-time / action-command battles (AI-controlled partner)
- Brains stat governs player control over moves
- Techniques / special moves (MP cost, element, power)
- Type and element advantages
- Status effects (poison, paralysis, sleep, confusion, etc.)
- Experience for stat gains (not traditional XP levels)
- Recruitment / post-battle befriending
- Fleeing, KO, and battle rewards (Bits, items, recruitment)

### Stats & Leveling
- Training-based stat growth (not XP levels)
- Training facilities / mini-games feeding stats
- Stat caps per stage
- Tiredness and happiness gating training effectiveness
- Bonus stat inheritance across generations

### Digimon AI (Partner)
- Autonomous behavior in battle and overworld
- Obedience scaling with Brains and Discipline
- Idle behaviors (wandering, reacting, sleeping, pooping)
- Hunger/fatigue-driven requests

### Enemy AI
- Overworld encounter behaviors (patrol, chase, flee)
- Battle AI (move selection, targeting, retreat thresholds)
- Boss AI patterns
- Recruitable NPC Digimon logic (conditions to join the city)

### Player Movement
- Third-person overworld traversal
- Partner Digimon follow behavior
- Interact / inspect prompts
- Zone transitions between map areas
- Camera control

### World & Map System
- File Island overworld with discrete zones
- Zone loading/unloading
- Waypoints and fast travel (if applicable)
- Environmental interactables (trees, signs, items)
- Day/night cycle

### Dialogue System
- Branching dialogue trees
- NPC speaker data (portrait placeholder, name, voice cue)
- Choice-driven responses
- Conditional dialogue based on quest/world state
- Text localization hook

### Cutscene System
- Scripted sequences (camera, actors, dialogue, SFX)
- Skippable cutscenes
- Triggered cutscenes (zone entry, quest milestone, evolution)
- Timeline-style authoring

### Quest System
- Main story quests
- Side quests / recruitment quests
- Quest states (inactive, active, completed, failed)
- Objectives and subtasks
- Rewards (items, recruits, world changes)
- Quest log UI

### Recruitment / City-Building
- Each recruited Digimon expands File City
- City services unlocked by recruits (shop, clinic, farm, gym, etc.)
- Recruit prerequisites and triggers
- City progression tracker

### Item & Inventory System
- Item categories (food, medicine, training, key items, evolution items)
- Stackable inventory with cap
- Using items on partner (feeding, healing, training aids)
- Shop buy/sell
- Equipment / accessory slots (if applicable)
- Bits (currency)

### Care System
- Feeding (hunger, weight, poop)
- Sleeping / resting (tiredness)
- Bathroom / cleanliness (care mistakes if ignored)
- Praise and scold (discipline, happiness)
- Training sessions
- Illness and medicine

### Save / Load System
- Multiple save slots
- Autosave on key events
- Serialization of Digimon state, world state, quests, inventory, time
- Save file versioning / migration

### UI & Menus
- Main menu (new game, load, options)
- HUD (partner status, time, minimap, currency)
- Pause menu
- Status / Digimon info screen
- Inventory screen
- Quest log
- Recruitment roster / city view
- Shop UI
- Dialogue UI
- Battle UI (HP/MP, commands, tech list)
- Settings (audio, controls, graphics)
- Save/load screen

### Audio System
- Music playback with zone-based tracks
- Crossfade between tracks
- SFX bus (UI, combat, ambient, voice cues)
- Volume mixer (master/music/SFX)
- One-shot and looped SFX helpers

### Time System
- In-game clock (minutes/hours/days)
- Day/night cycle tied to world and AI
- Digimon aging and lifespan tied to real/game time
- Time-gated events (shops open, NPCs move)

### Input System
- Keyboard + mouse
- Gamepad support
- Rebindable controls
- Context-sensitive prompts

### Settings & Config
- Graphics, audio, controls
- Language selection hook
- Accessibility options (text speed, subtitles)

### Debug / Developer Tools
- Debug console
- Cheat menu (spawn, evolve, teleport, fast-forward time)
- Stat inspector overlay
- Save state viewer

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
