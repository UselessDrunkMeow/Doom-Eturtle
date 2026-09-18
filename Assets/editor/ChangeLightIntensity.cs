using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// HDRP version. Put this file inside a folder named "Editor" (e.g. Assets/Editor/).
// Then use the menu: Tools > Lighting > Change Light Intensity (600 -> 200)
// Values are in whatever unit each light's Inspector shows (e.g. Lumen).
public static class ChangeLightIntensity
{
    private const float FromIntensity = 600f;
    private const float ToIntensity = 100f;
    private const float Tolerance = 0.05f; // unit conversion can add tiny rounding errors

    [MenuItem("Tools/Lighting/Change Light Intensity (600 -> 100)")]
    private static void Change()
    {
        int changed = 0;

        // Finds every Light in the open scenes, including ones on inactive GameObjects.
        foreach (Light light in Resources.FindObjectsOfTypeAll<Light>())
        {
            // Skip prefab assets and hidden/internal objects; only touch lights in scenes.
            if (EditorUtility.IsPersistent(light)) continue;
            if (!light.gameObject.scene.IsValid()) continue;
            if ((light.hideFlags & HideFlags.NotEditable) != 0) continue;

#if UNITY_6000_0_OR_NEWER
            // Unity 6: Light.intensity is stored in the light type's native unit
            // (e.g. Candela for point lights). Light.lightUnit is only the unit the
            // Inspector displays (e.g. Lumen), so convert before comparing/setting.
            var nativeUnit = UnityEngine.Rendering.LightUnitUtils.GetNativeLightUnit(light.type);
            float shown = UnityEngine.Rendering.LightUnitUtils.ConvertIntensity(
                light, light.intensity, nativeUnit, light.lightUnit);

            if (Mathf.Abs(shown - FromIntensity) > Tolerance) continue;

            Undo.RecordObject(light, "Change Light Intensity");
            light.intensity = UnityEngine.Rendering.LightUnitUtils.ConvertIntensity(
                light, ToIntensity, light.lightUnit, nativeUnit);
            EditorUtility.SetDirty(light);
#else
            // Older HDRP: the displayed intensity lives on HDAdditionalLightData.
            var hdLight = light.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
            if (hdLight == null) continue;
            if (Mathf.Abs(hdLight.intensity - FromIntensity) > Tolerance) continue;

            Undo.RecordObjects(new Object[] { hdLight, light }, "Change Light Intensity");
            hdLight.intensity = ToIntensity;
            EditorUtility.SetDirty(hdLight);
            EditorUtility.SetDirty(light);
#endif
            EditorSceneManager.MarkSceneDirty(light.gameObject.scene);
            Debug.Log($"Changed intensity {FromIntensity} -> {ToIntensity}: {GetPath(light.transform)}", light);
            changed++;
        }

        Debug.Log($"Done. Changed {changed} light(s) from {FromIntensity} to {ToIntensity}. Remember to save the scene.");
    }

    private static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}