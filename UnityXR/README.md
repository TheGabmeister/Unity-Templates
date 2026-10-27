# UnityXR

A Unity 6 (`6000.5.11f1`) mixed-reality project template built on Unity's MR template. It uses URP, OpenXR (Android XR and Meta OpenXR), AR Foundation, XR Interaction Toolkit 3.5, XR Hands and Composition Layers. XR Simulation is configured for testing in the Editor.

## Project layout
- `Assets/_Project/` holds this project's own content (Scripts, Prefabs, Scenes, Models, Shaders, Editor). It is empty except for `Scenes/TestScene.unity`.
- `Assets/MRTemplateAssets/` is Unity's MR template content. Its scripts compile into the `MRTemplate` assembly (`MRTemplate.asmdef`) under the namespace `UnityEngine.XR.Templates.MR`. The asmdef defines `USING_META_OCCLUSION` when `com.unity.xr.meta-openxr` >= 2.1.0.
- `Assets/Scenes/SampleScene.unity` is the only scene in Build Settings. It wires together the template's main systems:
  - `GoalManager` runs onboarding.
  - `ARFeatureController` toggles passthrough, planes, bounding boxes and occlusion.
  - `SpawnedObjectsManager` spawns objects with persistent anchors.
- `Assets/Samples/` holds package samples (XRI Starter Assets, AR Starter Assets, Hands Interaction Demo, XR Interaction Simulator, XR Hands HandVisualizer). Each sample sits in a folder named for its package version, and re-importing a sample overwrites it, so don't edit samples in place.
- `Assets/XR/`, `Assets/XRI/` and `Assets/Settings/` hold the XR loader (OpenXR and Simulation), XRI and URP config assets.

## XR modes (Editor Play mode)
**Tools > XR Mode** switches what Editor Play mode runs against. The checkmark shows the current mode. You can't switch during Play mode. The script is `Assets/_Project/Editor/XRModeMenu.cs`.

| Mode | Standalone XR loader | XR Interaction Simulator | Use it for |
|---|---|---|---|
| **Simulation** (default) | XR Simulation | Spawned on Play | Testing without a headset. Keyboard and mouse drive the head, controllers and hands. |
| **Link** | OpenXR | Off | A Quest connected through Meta Horizon Link. |

- Only the Standalone (Editor) loader changes. Android builds always use OpenXR.
- Don't run Link mode with the simulator enabled. The simulator removes real headsets from the Input System and stops real hand tracking, so the Quest would show the image but not track.
- The mode is saved in `Assets/XR/XRGeneralSettingsPerBuildTarget.asset` and `Assets/XRI/Settings/Resources/XRDeviceSimulatorSettings.asset`, so a switch shows up in git. Switch back to Simulation before committing.
- From the CLI: `unity command menu -- --path "Tools/XR Mode/Simulation"` (or `.../Link`).
