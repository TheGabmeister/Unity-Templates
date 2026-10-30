# AGENTS.md

## Session startup check
- At the start of every new session, before doing any task, verify that Unity skills are installed and that the Unity CLI is available and working by running `unity --version`.
- If Unity skills are missing or the Unity CLI check fails, stop all task work and tell the user to finish the Unity skills and Unity CLI setup first before you can perform tasks.

## Coding convention
- Follow KISS and YAGNI.  Use DRY when something repeats 3 or more times.
- Follow Unity architecture best practices.

## Build profiles
- Never create a build profile (no `--create-profile`, no `build create_profile`, nothing under `Assets/Settings/Build Profiles/`), including for Meta Quest.
- Build Quest apps from the default Android platform with Project Settings: `unity build --target Android -o Builds/<Name>.apk`. The OpenXR Meta Quest feature already adds hand tracking, passthrough and Quest device support, and a profile only adds a second copy of the Player and Quality settings that drifts from Project Settings.

## Project information
Read [README.md](README.md) for the project overview and layout. Keep all project-related information there, not in this file.