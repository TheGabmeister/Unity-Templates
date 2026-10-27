using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Management;

namespace UnityXR.EditorTools
{
    /// <summary>
    /// Switches what Editor Play mode runs against.
    /// Simulation: XR Simulation loader plus the XR Interaction Simulator (keyboard and mouse drive head and controllers).
    /// Link: OpenXR loader with no simulator, for a Quest connected through Meta Horizon Link.
    /// Only the Standalone (Editor) loader changes; Android builds are unaffected.
    /// </summary>
    static class XRModeMenu
    {
        const string k_SimulationMenu = "Tools/XR Mode/Simulation";
        const string k_LinkMenu = "Tools/XR Mode/Link";
        const string k_SimulationLoader = "UnityEngine.XR.Simulation.SimulationLoader";
        const string k_OpenXRLoader = "UnityEngine.XR.OpenXR.OpenXRLoader";
        // XRDeviceSimulatorSettings is internal to XRI, so it is edited through SerializedObject.
        const string k_SimulatorSettings = "XRDeviceSimulatorSettings";
        const string k_SimulatorPrefab = "XR Interaction Simulator";

        [MenuItem(k_SimulationMenu, priority = 0)]
        static void SetSimulation() => SetMode(simulation: true);

        [MenuItem(k_LinkMenu, priority = 1)]
        static void SetLink() => SetMode(simulation: false);

        [MenuItem(k_SimulationMenu, true)]
        [MenuItem(k_LinkMenu, true)]
        static bool Validate()
        {
            var simulation = IsSimulation();
            Menu.SetChecked(k_SimulationMenu, simulation);
            Menu.SetChecked(k_LinkMenu, !simulation);
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        static XRManagerSettings StandaloneManager =>
            XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone).Manager;

        static bool IsSimulation() =>
            StandaloneManager.activeLoaders.Any(l => l.GetType().FullName == k_SimulationLoader);

        static void SetMode(bool simulation)
        {
            var manager = StandaloneManager;
            XRPackageMetadataStore.RemoveLoader(manager, simulation ? k_OpenXRLoader : k_SimulationLoader, BuildTargetGroup.Standalone);
            XRPackageMetadataStore.AssignLoader(manager, simulation ? k_SimulationLoader : k_OpenXRLoader, BuildTargetGroup.Standalone);

            var settingsAsset = Resources.Load<ScriptableObject>(k_SimulatorSettings);
            if (settingsAsset == null)
            {
                Debug.LogWarning($"{k_SimulatorSettings} asset not found. Open Project Settings > XR Plug-in Management > XR Interaction Toolkit once to create it.");
            }
            else
            {
                var settings = new SerializedObject(settingsAsset);
                settings.FindProperty("m_AutomaticallyInstantiateSimulatorPrefab").boolValue = simulation;
                var prefab = settings.FindProperty("m_SimulatorPrefab");
                if (simulation && prefab.objectReferenceValue == null)
                    prefab.objectReferenceValue = FindSimulatorPrefab();
                settings.ApplyModifiedPropertiesWithoutUndo();
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"XR Mode: {(simulation ? "Simulation" : "Link")}");
        }

        static GameObject FindSimulatorPrefab()
        {
            var prefab = AssetDatabase.FindAssets($"\"{k_SimulatorPrefab}\" t:Prefab")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(go => go.TryGetComponent<XRInteractionSimulator>(out _));
            if (prefab == null)
                Debug.LogWarning($"{k_SimulatorPrefab} prefab not found. Import the XR Interaction Simulator sample from the XR Interaction Toolkit package.");
            return prefab;
        }
    }
}
