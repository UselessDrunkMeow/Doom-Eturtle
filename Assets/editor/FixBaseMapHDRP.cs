using UnityEngine;
using UnityEditor;

public class FixBaseMapHDRP : EditorWindow
{
    private const string TargetShader = "Universal Render Pipeline/Particles/Unlit";
    private const string HDRPUnlitParticles = "HDRP/Unlit";

    [MenuItem("Tools/HDRP/Fix Base Maps for Particles")]
    public static void FixParticleBaseMaps()
    {
        Shader hdrpShader = Shader.Find(HDRPUnlitParticles);
        if (hdrpShader == null)
        {
            Debug.LogError($"[HDRP Fixer] Could not find shader: '{HDRPUnlitParticles}'");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Fix Particle Base Maps Only");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null || mat.shader == null) continue;

            if (mat.shader.name == TargetShader || mat.shader.name == HDRPUnlitParticles)
            {
                // 1. Extract ONLY the BaseMap texture from URP
                Texture urpBaseMap = null;
                if (mat.HasProperty("_BaseMap"))
                {
                    urpBaseMap = mat.GetTexture("_BaseMap");
                }

                // 2. Assign HDRP Particle Unlit Shader if not already assigned
                Undo.RecordObject(mat, "Assign HDRP Base Map Only");
                if (mat.shader.name != HDRPUnlitParticles)
                {
                    mat.shader = hdrpShader;
                }

                // 3. Assign BaseMap directly to HDRP texture map slots
                if (urpBaseMap != null)
                {
                    if (mat.HasProperty("_UnlitColorMap")) mat.SetTexture("_UnlitColorMap", urpBaseMap);
                    if (mat.HasProperty("_BaseColorMap")) mat.SetTexture("_BaseColorMap", urpBaseMap);
                    
                    mat.EnableKeyword("_COLORMAP");

                    EditorUtility.SetDirty(mat);
                    count++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[HDRP Fixer] Successfully mapped URP BaseMap texture to HDRP Color Map for {count} material(s).");
    }
}