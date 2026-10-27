# UnityXR

A Unity 6 (`6000.5.11f1`) mixed-reality project template built on Unity's MR template. It uses URP, OpenXR (Android XR and Meta OpenXR), AR Foundation, XR Interaction Toolkit 3.5, XR Hands and Composition Layers. XR Simulation is configured for testing in the Editor.

## Project layout
- `Assets/_Project/` holds this project's own content (Scripts, Prefabs, Scenes, Models, Shaders, Editor). So far it has `Scenes/TestScene.unity`, `Prefabs/XR Rig.prefab` and the XR mode menu.
- `Assets/MRTemplateAssets/` is Unity's MR template content. Its scripts compile into the `MRTemplate` assembly (`MRTemplate.asmdef`) under the namespace `UnityEngine.XR.Templates.MR`. The asmdef defines `USING_META_OCCLUSION` when `com.unity.xr.meta-openxr` >= 2.1.0.
- `Assets/Scenes/SampleScene.unity` is the only scene in Build Settings. It wires together the template's main systems:
  - `GoalManager` runs onboarding.
  - `ARFeatureController` toggles passthrough, planes, bounding boxes and occlusion.
  - `SpawnedObjectsManager` spawns objects with persistent anchors.
- `Assets/Samples/` holds package samples (XRI Starter Assets, AR Starter Assets, Hands Interaction Demo, XR Interaction Simulator, XR Hands HandVisualizer). Each sample sits in a folder named for its package version, and re-importing a sample overwrites it, so don't edit samples in place.
- `Assets/XR/`, `Assets/XRI/` and `Assets/Settings/` hold the XR loader (OpenXR and Simulation), XRI and URP config assets.

## XR Rig prefab
`Assets/_Project/Prefabs/XR Rig.prefab` is a minimal rig for new scenes. It uses only package components and its own input bindings, so it doesn't depend on `Assets/Samples/` or `Assets/MRTemplateAssets/`. Drop it into a scene that has no other camera, XR Origin or AR Session.

- **XR Origin**: the Main Camera is set up for passthrough (transparent black background, AR Camera Manager and Background), and the Camera Y Offset matches the MR template.
- **Left and Right Controller**: each has a Near-Far Interactor (grip grabs, trigger activates and clicks UI), a ray line and a small sphere as the visual.
- **XR Interaction Manager, EventSystem (XR UI Input Module) and AR Session.**
- It has no hand tracking, locomotion, poke or AR plane features. For those, use `MR Interaction Setup` from `Assets/MRTemplateAssets/Prefabs/`, as `SampleScene` does.

## XR modes (Editor Play mode)
**Tools > XR Mode** switches what Editor Play mode runs against. The checkmark shows the current mode. You can't switch during Play mode. The script is `Assets/_Project/Editor/XRModeMenu.cs`.

| Mode | Standalone XR loader | XR Interaction Simulator | Use it for |
|---|---|---|---|
| **Simulation** (default) | XR Simulation | Spawned on Play | Testing without a headset. Keyboard and mouse drive the head, controllers and hands. |
| **Link** | OpenXR | Off | A Quest connected through Meta Horizon Link. |

- Only the Standalone (Editor) loader changes. Android builds always use OpenXR.
- Don't run Link mode with the simulator enabled. The simulator removes real headsets from the Input System and stops real hand tracking, so the Quest would show the image but not track.
- The mode is saved in `Assets/XR/XRGeneralSettingsPerBuildTarget.asset` and `Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset`, so a switch shows up in git. Switch back to Simulation before committing.
- From the CLI: `unity command editor.invoke_menu -- --path "Tools/XR Mode/Simulation"` (or `.../Link`).
