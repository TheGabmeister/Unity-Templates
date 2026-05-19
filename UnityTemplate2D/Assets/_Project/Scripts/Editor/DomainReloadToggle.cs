using UnityEditor;
using UnityEditor.Toolbars;

public class DomainReloadToggle
{
    const string k_Path = "DomainReload/Toggle";

    [MainToolbarElement(k_Path, defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateToggle()
    {
        bool enabled = IsDomainReloadEnabled();
        var content = new MainToolbarContent(
            enabled ? "DR: On" : "DR: Off",
            "Domain Reload\nEnabled: Reload Domain and Scene\nDisabled: Reload Scene Only"
        );

        return new MainToolbarToggle(content, enabled, isOn =>
        {
            if (isOn)
                EditorSettings.enterPlayModeOptionsEnabled = false;
            else
            {
                EditorSettings.enterPlayModeOptionsEnabled = true;
                EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            }

            MainToolbar.Refresh(k_Path);
        });
    }

    static bool IsDomainReloadEnabled()
    {
        return !EditorSettings.enterPlayModeOptionsEnabled
            || !EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
    }
}
