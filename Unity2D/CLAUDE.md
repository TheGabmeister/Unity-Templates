# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Session startup check
At the start of every session, before doing any other work:
1. Check that the Unity CLI works (e.g. run `unity --version`).
2. Check that the Unity skills (`unity:*`, e.g. `unity:unity-cli`) are installed and available.

If either check fails, tell the user what's missing so they can finish this setup, then continue with the task.

## Coding convention
- Follow KISS and YAGNI.  Use DRY when something repeats 3 or more times.
- Follow Unity architecture best practices.
