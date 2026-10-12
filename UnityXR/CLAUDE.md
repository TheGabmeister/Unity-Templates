# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Session startup check
At the start of every session, before doing any other work:
1. Check that the Unity CLI works (e.g. run `unity --version`).
2. Check that the Unity skills (`unity:*`, e.g. `unity:unity-cli`) are installed and available.

If either check fails, stop whatever you're doing and tell the user to finish this setup first. Do not continue with the task until it's resolved.

## Coding convention
- Follow KISS and YAGNI.  Use DRY when something repeats 3 or more times.
- Follow Unity architecture best practices.

## Project overview
A Unity 6 (`6000.5.11f1`) mixed-reality project template built on Unity's MR template. It uses URP, OpenXR (Android XR and Meta OpenXR), AR Foundation, XR Interaction Toolkit 3.5, XR Hands and Composition Layers. XR Simulation is configured for testing in the Editor. The sibling folders `../Unity2D` and `../Unity3D` are separate templates.

## Commands
Run these from the project root (`UnityXR/`). The CLI reads the Editor version from `ProjectSettings/ProjectVersion.txt`.
- Open the project: `unity open`
- Recompile the running Editor and report compile errors: `unity recompile`
- Run tests: `unity test --mode EditMode` or `unity test --mode PlayMode`. Results go to `test-results.xml`.
- Run a single test: `unity test --filter <TestName>`
- Build for Android: `unity build --target Android -o <output.apk>`. Meta Quest and other partner platforms build from a build profile (`--create-profile`, then `--profile`). No profiles exist yet.

The Test Framework package is installed, but the project has no tests yet.

## Project layout
- `Assets/_Project/` holds this project's own content (Scripts, Prefabs, Scenes, Models, Shaders, Editor). It is empty except for `Scenes/TestScene.unity`.
- `Assets/MRTemplateAssets/` is Unity's MR template content. Its scripts compile into the `MRTemplate` assembly (`MRTemplate.asmdef`) under the namespace `UnityEngine.XR.Templates.MR`. The asmdef defines `USING_META_OCCLUSION` when `com.unity.xr.meta-openxr` >= 2.1.0.
- `Assets/Scenes/SampleScene.unity` is the only scene in Build Settings. It wires together the template's main systems:
  - `GoalManager` runs onboarding.
  - `ARFeatureController` toggles passthrough, planes, bounding boxes and occlusion.
  - `SpawnedObjectsManager` spawns objects with persistent anchors.
- `Assets/Samples/` holds package samples (XRI Starter Assets, AR Starter Assets, Hands Interaction Demo, XR Hands HandVisualizer). Each sample sits in a folder named for its package version, and re-importing a sample overwrites it, so don't edit samples in place.
- `Assets/XR/`, `Assets/XRI/` and `Assets/Settings/` hold the XR loader (OpenXR and Simulation), XRI and URP config assets.