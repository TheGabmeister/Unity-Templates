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
- Build for Android: `unity build --target Android -o <output.apk>`. Meta Quest and other partner platforms build from a build profile (`--create-profile`, then `--profile`). No profiles exist yet.

The Test Framework package is installed, but the project has no tests yet.