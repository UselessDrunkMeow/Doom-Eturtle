// Switches the project from the offline Path Tracer (whose OptiX/OIDN denoiser only runs
// on a fully converged, static frame) to real-time ray-traced effects that have their own
// per-frame denoisers and work while the camera moves.
//
// Runs once automatically after this script compiles. Re-run any time via
// Tools > Doom-Eturtle > Set Up Real-Time Ray Tracing.
// Backups of the original profiles are in <project>/_backup_volume_profiles.

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[InitializeOnLoad]
public static class SetupRealtimeRayTracing
{
    // Volume profile used by Level Scene and MainMenu (Creepy_Cat .../DefaultVolumeProfile.asset)
    const string SceneProfileGuid = "c74d1d9e01a746b46a9175a027c58b9d";
    // HDRP global default volume profile (HDRPDefaultResources/DefaultSettingsVolumeProfile.asset)
    const string DefaultProfileGuid = "bc16d01adcbf344ca862b42c044da1e0";

    const string DoneKey = "DoomEturtle.RealtimeRTSetup.v1";

    static SetupRealtimeRayTracing()
    {
        if (EditorPrefs.GetBool(DoneKey, false)) return;
        EditorApplication.delayCall += () =>
        {
            if (Apply()) EditorPrefs.SetBool(DoneKey, true);
        };
    }

    [MenuItem("Tools/Doom-Eturtle/Set Up Real-Time Ray Tracing")]
    static void ApplyFromMenu() => Apply();

    static bool Apply()
    {
        var sceneProfile = Load(SceneProfileGuid);
        var defaultProfile = Load(DefaultProfileGuid);
        if (sceneProfile == null || defaultProfile == null)
        {
            Debug.LogError("[RT Setup] Could not find the volume profiles, nothing changed.");
            return false;
        }

        // 1) Turn the offline path tracer off (both in the scene profile and the global default,
        //    otherwise the default's "enabled" would still apply). Denoiser settings are kept,
        //    so you can switch it back on for screenshots/recordings.
        DisablePathTracing(defaultProfile);
        DisablePathTracing(sceneProfile);

        // 2) Real-time ray-traced global illumination (denoised every frame)
        var gi = GetOrAdd<GlobalIllumination>(sceneProfile);
        gi.enable.Override(true);
        gi.tracing.Override(RayCastingMode.RayTracing);
        gi.mode.Override(RayTracingMode.Performance);
        gi.quality.Override((int)ScalableSettingLevelParameter.Level.Medium);

        // 3) Real-time ray-traced reflections (denoised every frame)
        var ssr = GetOrAdd<ScreenSpaceReflection>(sceneProfile);
        ssr.enabled.Override(true);
        ssr.tracing.Override(RayCastingMode.RayTracing);
        ssr.mode.Override(RayTracingMode.Performance);
        ssr.quality.Override((int)ScalableSettingLevelParameter.Level.Medium);

        // 4) Ray-traced ambient occlusion (denoised every frame)
        var ao = GetOrAdd<ScreenSpaceAmbientOcclusion>(sceneProfile);
        ao.intensity.Override(1f);
        ao.rayTracing.Override(true);
        ao.quality.Override((int)ScalableSettingLevelParameter.Level.Medium);

        EditorUtility.SetDirty(sceneProfile);
        EditorUtility.SetDirty(defaultProfile);
        AssetDatabase.SaveAssets();

        Debug.Log("[RT Setup] Done: Path Tracing off; ray-traced GI, reflections and AO on (Performance mode, " +
                  "denoised per frame). Ray tracing only runs on Windows/DX12 with an RTX-class GPU; on Mac " +
                  "HDRP falls back to the regular screen-space versions. Rebuild the Windows player to see it.");
        return true;
    }

    static void DisablePathTracing(VolumeProfile profile)
    {
        if (profile.TryGet(out PathTracing pt))
            pt.enable.Override(false);
    }

    static T GetOrAdd<T>(VolumeProfile profile) where T : VolumeComponent
    {
        if (profile.TryGet(out T c))
        {
            c.active = true;
            return c;
        }
        c = profile.Add<T>(false);
        c.name = typeof(T).Name;
        c.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(c, profile);
        return c;
    }

    static VolumeProfile Load(string guid)
    {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
    }
}
