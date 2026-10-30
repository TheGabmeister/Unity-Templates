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

## Project information
Read [README.md](README.md) for the project overview and layout. Keep all project-related information there, not in this file.

## Commands
Run these from the project root (`UnityXR/`). The CLI reads the Editor version from `ProjectSettings/ProjectVersion.txt`.
- Open the project: `unity open`
- Recompile the running Editor and report compile errors: `unity recompile`
- Run tests: `unity test --mode EditMode` or `unity test --mode PlayMode`. Results go to `test-results.xml`.
- Run a single test: `unity test --filter <TestName>`
- Build for Quest (Editor closed): `unity build --target Android -o Builds/<Name>.apk`. With the Editor open: `unity command build run -- --target Android --outputPath Builds/<Name>.apk --confirm true`, then poll `unity command build status` until `completed`.

## Build profiles
Never create a build profile (no `--create-profile`, no `build create_profile`, nothing under `Assets/Settings/Build Profiles/`), including for Meta Quest. Build Quest apps from the default Android platform with Project Settings. The OpenXR Meta Quest feature already adds hand tracking, passthrough and Quest device support, and a profile only adds a second copy of the Player and Quality settings that drifts from Project Settings.

The Test Framework package is installed, but the project has no tests yet.