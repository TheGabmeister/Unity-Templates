using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[InitializeOnLoad]
static class DomainReloadToggle
{
    const string k_ToggleName = "domain-reload-toggle";

    static DomainReloadToggle()
    {
        EditorApplication.update += TryAttach;
    }

    static void TryAttach()
    {
        var toolbar = FindToolbar();
        if (toolbar == null) return;

        var root = toolbar.rootVisualElement;
        if (root == null) return;
        if (root.Q(k_ToggleName) != null)
        {
            EditorApplication.update -= TryAttach;
            return;
        }

        var playMode = root.Q("PlayMode");
        if (playMode == null) return;

        EditorApplication.update -= TryAttach;

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
