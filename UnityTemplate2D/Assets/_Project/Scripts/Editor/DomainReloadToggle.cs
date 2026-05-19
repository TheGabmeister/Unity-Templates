using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

/*
Adds a "DR: On/Off" toggle to the main toolbar next to the Play button.
Controls whether entering Play Mode reloads the domain (slow, full reset)
or only reloads the scene (fast iteration, but static state persists).

Why reflection instead of MainToolbarElementAttribute?
Unity 6.3 has an official MainToolbarElement API, but elements registered
through it are hidden by default and require the user to right-click the
toolbar to enable them. This approach injects directly into the toolbar's
VisualElement tree so the toggle is always visible without manual setup.

Why InitializeOnLoad?
The toolbar UI (MainToolbarWindow and its "PlayMode" VisualElement) already
exists when InitializeOnLoad runs, so we can attach directly without
deferring via delayCall or EditorApplication.update.
*/

[InitializeOnLoad]
static class DomainReloadToggle
{
    const string k_ToggleName = "domain-reload-toggle";

    static DomainReloadToggle()
    {
        var toolbar = FindToolbar();
        if (toolbar == null) return;

        var root = toolbar.rootVisualElement;
        if (root == null) return;
        if (root.Q(k_ToggleName) != null) return;

        var playMode = root.Q("PlayMode");
        if (playMode == null) return;

        var toggle = new ToolbarToggle
        {
            name = k_ToggleName,
            text = IsDomainReloadEnabled() ? "DR: On" : "DR: Off",
            value = IsDomainReloadEnabled(),
            tooltip = "Domain Reload\nEnabled: Reload Domain and Scene\nDisabled: Reload Scene Only",
            style = { alignSelf = Align.Center }
        };
        ApplyToggleStyle(toggle, toggle.value);
        toggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
                EditorSettings.enterPlayModeOptionsEnabled = false;
            else
            {
                EditorSettings.enterPlayModeOptionsEnabled = true;
                EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            }
            toggle.text = evt.newValue ? "DR: On" : "DR: Off";
            ApplyToggleStyle(toggle, evt.newValue);
        });

        var parent = playMode.parent;
        parent.Insert(parent.IndexOf(playMode) + 1, toggle);
    }

    // MainToolbarWindow is internal, so we find it by name via reflection.
    // Falls back to "Toolbar" for pre-6.3 Unity versions.
    static EditorWindow FindToolbar()
    {
        var assembly = typeof(Editor).Assembly;
        var toolbarType = assembly.GetType("UnityEditor.MainToolbarWindow")
            ?? assembly.GetType("UnityEditor.Toolbar");
        if (toolbarType == null) return null;
        return Resources.FindObjectsOfTypeAll(toolbarType).FirstOrDefault() as EditorWindow;
    }

    static void ApplyToggleStyle(VisualElement element, bool enabled)
    {
        element.style.opacity = enabled ? 1f : 0.6f;
    }

    static bool IsDomainReloadEnabled()
    {
        return !EditorSettings.enterPlayModeOptionsEnabled
            || !EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
    }
}
