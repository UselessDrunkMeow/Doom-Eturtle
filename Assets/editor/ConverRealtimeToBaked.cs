using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Put this file inside a folder named "Editor" (e.g. Assets/Editor/).
// Then use the menu: Tools > Lighting > Convert Baked Lights To Realtime
public static class ConvertBakedLightsToRealtime
{
    [MenuItem("Tools/Lighting/Convert Baked Lights To Realtime")]
    private static void ConvertBaked()
    {
        Convert(includeMixed: false);
    }

    [MenuItem("Tools/Lighting/Convert Baked + Mixed Lights To Realtime")]
    private static void ConvertBakedAndMixed()
    {
        Convert(includeMixed: true);
    }

    private static void Convert(bool includeMixed)
    {
        int converted = 0;

        // Finds every Light in the open scenes, including ones on inactive GameObjects.
        foreach (Light light in Resources.FindObjectsOfTypeAll<Light>())
        {
            // Skip prefab assets and hidden/internal objects; only touch lights in scenes.
            if (EditorUtility.IsPersistent(light)) continue;
            if (!light.gameObject.scene.IsValid()) continue;
            if ((light.hideFlags & HideFlags.NotEditable) != 0) continue;

            bool isBaked = light.lightmapBakeType == LightmapBakeType.Baked;
            bool isMixed = light.lightmapBakeType == LightmapBakeType.Mixed;

            if (isBaked || (includeMixed && isMixed))
            {
                Undo.RecordObject(light, "Convert Lights To Realtime");
                light.lightmapBakeType = LightmapBakeType.Realtime;
                EditorUtility.SetDirty(light);
                EditorSceneManager.MarkSceneDirty(light.gameObject.scene);

                Debug.Log($"Converted to realtime: {GetPath(light.transform)}", light);
                converted++;
            }
        }

        Debug.Log($"Done. Converted {converted} light(s) to Realtime. " +
                  "Remember to save the scene, and clear or rebake lighting data.");
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