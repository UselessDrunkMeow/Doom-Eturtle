using UnityEngine;
using UnityEditor;

public class Convert2DToHDRPLit : EditorWindow
{
    private const string TargetShaderName = "Universal Render Pipeline/2D/Mesh2D-Lit-Default";
    private const string NewShaderName = "HDRP/Lit";

    [MenuItem("Tools/HDRP/Convert 2D Materials to HDRP Lit")]
    public static void ConvertMaterials()
    {
        // Load the replacement HDRP shader
        Shader hdrpLitShader = Shader.Find(NewShaderName);
        if (hdrpLitShader == null)
        {
            Debug.LogError($"[HDRP Converter] Could not find shader: {NewShaderName}. Ensure HDRP is installed in your project.");
            return;
        }

        // Find all Material asset GUIDs in the project
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int updatedCount = 0;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Convert 2D Materials to HDRP Lit");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat != null && mat.shader != null && mat.shader.name == TargetShaderName)
            {
                Undo.RecordObject(mat, "Change Material Shader");

                // Try to capture the main texture assigned in the 2D shader
                Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;

                // Assign the HDRP shader
                mat.shader = hdrpLitShader;

                // Reassign texture to HDRP's Base Color map property
                if (mainTex != null && mat.HasProperty("_BaseColorMap"))
                {
                    mat.SetTexture("_BaseColorMap", mainTex);
                }

                EditorUtility.SetDirty(mat);
                updatedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[HDRP Converter] Successfully converted {updatedCount} material(s) from '{TargetShaderName}' to '{NewShaderName}'.");
    }
}