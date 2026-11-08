# AGENTS.md

This file provides guidance to AI coding agents working with code in this repository.

## Session startup check
At the start of every session, before doing any other work, check that the Unity CLI works (e.g. run `unity --version`). If it fails, tell the user so they can finish the Unity CLI setup, then continue with the task.

## Coding convention
- Follow KISS and YAGNI.  Use DRY when something repeats 3 or more times.
- Follow Unity architecture best practices.

## Project information
Read [README.md](README.md) for the project overview and layout. Keep all project-related information there, not in this file.

## Commands
Run these from the project root (`UnityXR/`). The CLI reads the Editor version from `ProjectSettings/ProjectVersion.txt`.
- Open the project: `unity open`
- Recompile the running Editor and report compile errors: `unity recompile`
- Run tests: `unity test --mode EditMode` or `unity test --mode PlayMode`. Results go to `test-results.xml`. The Test Framework package is installed, but the project has no tests yet.
- Run a single test: `unity test --filter <TestName>`
- Build for Quest (Editor open): `unity command build run -- --target Android --output-path Builds/<Name>.apk --confirm true`, then poll `unity command build status` until `completed`. It switches the active platform to Android if needed. `unity build --target Android` doesn't work: the CLI refuses non-desktop targets without `--profile` or `--execute-method`.

## Build profiles
Never create a build profile (no `--create-profile`, no `build create_profile`, nothing under `Assets/Settings/Build Profiles/`), including for Meta Quest. Build Quest apps from the default Android platform with Project Settings. The OpenXR Meta Quest feature already adds hand tracking, passthrough and Quest device support, and a profile only adds a second copy of the Player and Quality settings that drifts from Project Settings.
