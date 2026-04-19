# SPEC — Phase 0: Foundation

High-level spec for the five plumbing systems that everything else depends on. No gameplay features — this is the scaffolding.

## Goal

A runnable Unity project where you can press Play, see a debug console and an FPS overlay, hear a test SFX and music track through a mixer, load a blank "test zone" scene asynchronously with a loading screen, and swap between UI screens — all driven by the new Input System.

## What's in Phase 0

1. Input System
2. Audio System (bare)
3. UI Framework
4. Debug Tools (bare)
5. Scene / Zone Loader

## 1. Input System

**Purpose:** One place that owns "what the player pressed". Everything else listens.

- Unity's new Input System package
- Action maps: `Gameplay`, `UI`, `Debug`
- Core actions stubbed now: `Move`, `Look`, `Interact`, `Pause`, `ToggleDebug`
- Device support: keyboard + mouse, gamepad
- Rebinding is a hook only — no UI yet
- Exposed as a single service so other systems don't touch the raw API

**Done when:** A test script logs action events for all core actions on both keyboard and gamepad.

## 2. Audio System (bare)

**Purpose:** Play sounds and music without anyone hardcoding `AudioSource.Play`.

- One `AudioMixer` asset with buses: `Master`, `Music`, `SFX`, `UI`
- `AudioService` with `PlaySFX(clip)`, `PlayMusic(clip)`, `StopMusic()`, `SetBusVolume(bus, 0–1)`
- Simple one-shot pool for SFX
- Music player supports crossfade (stub — hard cut is fine for Phase 0 if crossfade slips)
- No zone-based music yet (that's Phase 6's full pass)

**Done when:** A debug hotkey plays a test SFX and starts a test music track on the correct buses.

## 3. UI Framework

**Purpose:** A consistent way to show/hide screens without piles of `SetActive` calls.

- Root `Canvas` + `EventSystem` in a persistent bootstrap scene
- `ScreenManager` with a stack: `Push`, `Pop`, `Replace`
- `BaseScreen` class with `OnShow`, `OnHide`, `OnBack`
- Input routing: `UI` action map enabled while any screen is up
- Placeholder screens: `LoadingScreen`, `PauseScreen` (empty except a title)
- Simple fade in/out transition

**Done when:** Pressing `Pause` pushes `PauseScreen`, pressing it again pops it, and fades work.

## 4. Debug Tools (bare)

**Purpose:** See what's happening at runtime without attaching a debugger.

- Toggleable overlay (`ToggleDebug` action)
- FPS counter + frame time
- In-game console: log capture, filter by level, command registry
- A few starter commands: `help`, `clear`, `time.scale <x>`, `scene.load <name>`
- Not shipped in release builds (compile guard)

**Done when:** Toggling the overlay shows FPS and the console; `help` lists registered commands.

## 5. Scene / Zone Loader

**Purpose:** Move between scenes without frame hitches or missing references.

- `SceneService` with `LoadZoneAsync(zoneId)` and `UnloadZoneAsync(zoneId)`
- Zones defined as `ZoneDefinition` ScriptableObjects (id, scene reference, display name)
- Additive loading: a persistent `Bootstrap` scene always stays loaded (holds services + UI root)
- `LoadingScreen` shown during transitions, driven by the UI Framework
- Progress reporting (0–1) to the loading screen
- One test zone: `Zone_TestRoom` — empty, just a ground plane and a light

**Done when:** A debug command `scene.load Zone_TestRoom` unloads the current zone, shows the loading screen, loads the new zone additively, and hides the loading screen.

## Project Layout

```
Assets/
  _Project/
    Bootstrap/            # Bootstrap scene + bootstrapper
    Scenes/
      Bootstrap.unity
      Zone_TestRoom.unity
    Scripts/
      Core/               # Service locator / bootstrapper
      Input/
      Audio/
      UI/
        Framework/
        Screens/
      Debug/
      Scenes/
    Settings/
      Input/              # InputActions asset
      Audio/              # AudioMixer asset
      Zones/              # ZoneDefinition assets
```

## Service Access

- A lightweight service locator on the `Bootstrap` scene
- Registered services: `IInputService`, `IAudioService`, `IUIService`, `ISceneService`, `IDebugService`
- No dependency injection framework — keep it simple

## Out of Scope for Phase 0

- Settings menu (Phase 6)
- Save/load (Phase 6)
- Rebind UI (Phase 6)
- Zone music selection (Phase 6)
- Any gameplay: no player, no camera, no partner, no Digimon

## Acceptance Checklist

- [ ] Project opens, `Bootstrap` scene is set as the first scene
- [ ] Pressing Play lands in `Zone_TestRoom` via the scene loader
- [ ] `ToggleDebug` shows FPS + console
- [ ] `help` lists commands; `scene.load Zone_TestRoom` works
- [ ] `Pause` pushes/pops `PauseScreen` with fade
- [ ] Debug hotkey plays SFX and music through the mixer
- [ ] Input events fire from both keyboard and gamepad
