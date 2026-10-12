# UnityXR

A Unity 6 (`6000.5.11f1`) mixed-reality project template built on Unity's MR template. It uses URP, OpenXR (Android XR and Meta OpenXR), AR Foundation, XR Interaction Toolkit 3.5, XR Hands and Composition Layers. XR Simulation is configured for testing in the Editor. The sibling folders `../Unity2D` and `../Unity3D` are separate templates.

## Project layout
- `Assets/_Project/` holds this project's own content (Scripts, Prefabs, Scenes, Models, Shaders, Editor). It is empty except for `Scenes/TestScene.unity`.
- `Assets/MRTemplateAssets/` is Unity's MR template content. Its scripts compile into the `MRTemplate` assembly (`MRTemplate.asmdef`) under the namespace `UnityEngine.XR.Templates.MR`. The asmdef defines `USING_META_OCCLUSION` when `com.unity.xr.meta-openxr` >= 2.1.0.
- `Assets/Scenes/SampleScene.unity` is the only scene in Build Settings. It wires together the template's main systems:
  - `GoalManager` runs onboarding.
  - `ARFeatureController` toggles passthrough, planes, bounding boxes and occlusion.
  - `SpawnedObjectsManager` spawns objects with persistent anchors.
- `Assets/Samples/` holds package samples (XRI Starter Assets, AR Starter Assets, Hands Interaction Demo, XR Hands HandVisualizer). Each sample sits in a folder named for its package version, and re-importing a sample overwrites it, so don't edit samples in place.
- `Assets/XR/`, `Assets/XRI/` and `Assets/Settings/` hold the XR loader (OpenXR and Simulation), XRI and URP config assets.
