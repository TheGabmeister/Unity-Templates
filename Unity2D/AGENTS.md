# AGENTS.md

## Session startup check
- At the start of every new session, before doing any task, verify that Unity skills are installed and that the Unity CLI is available and working by running `unity --version`.
- If Unity skills are missing or the Unity CLI check fails, tell the user what's missing so they can finish the Unity skills and Unity CLI setup, then continue with the task.

## Coding convention
- Follow KISS and YAGNI.  Use DRY when something repeats 3 or more times.
- Follow Unity architecture best practices.
