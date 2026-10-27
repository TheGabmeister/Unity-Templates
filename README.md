# Unity Game Project Templates
My personal Unity game project templates

## AI agent setup
Every template here is set up for Claude Code to drive the Unity Editor through the Unity CLI. Do these two steps once per computer.

### 1. Install the Unity CLI
In PowerShell, then open a new terminal and check it works:

```powershell
irm https://unity.com/install.ps1 | iex
unity --version
```

### 2. Install the Unity plugin from Unity's marketplace
The plugin provides the Unity skills (`unity:unity-cli`, `unity:ui`, `unity:urp-postprocessing` and the rest), so there's nothing separate to install for them. Don't use `unity skill install` as well: it adds a second copy of `unity-cli`.

Don't install **Unity** from the Claude Desktop plugins window. That listing comes from Anthropic's plugin directory and lags behind Unity's own release, so its instructions can use Editor commands that no longer exist. Install it from Unity's marketplace (`unity@unity-agent-plugin`) instead, in one of these ways:

- **Claude Desktop:** add these entries to `%USERPROFILE%\.claude\settings.json`, keeping anything already there:

  ```json
  {
    "extraKnownMarketplaces": {
      "unity-agent-plugin": {
        "source": { "source": "github", "repo": "unity-technologies/unity-agent-plugin" },
        "autoUpdate": true
      }
    },
    "enabledPlugins": {
      "unity@unity-agent-plugin": true
    }
  }
  ```

- **Claude Code in a terminal:** run `/plugin marketplace add unity-technologies/unity-agent-plugin`, then `/plugin install unity@unity-agent-plugin`.

Then start a new session. The plugin installs at session start, so it's available from the session after that.

If the Anthropic listing is already installed, uninstall it first, so you don't have two copies of the same skills.
