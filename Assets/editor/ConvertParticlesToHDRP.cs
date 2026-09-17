using UnityEngine;
using UnityEditor;

public class ConvertURPParticlesToHDRP_Advanced : EditorWindow
{
    // Add more (urpShader -> hdrpShader) pairs here if you use other
    // particle shader variants (e.g. "Simple Lit").
    private static readonly (string urpShader, string hdrpShader)[] ShaderMap =
    {
        ("Universal Render Pipeline/Particles/Unlit", "HDRP/Unlit"),
        ("Universal Render Pipeline/Particles/Lit", "HDRP/Lit"),
    };

    [MenuItem("Tools/HDRP/Convert URP Particles to HDRP (Advanced)")]
    public static void ConvertParticleMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Convert URP Particles to HDRP Advanced");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null || mat.shader == null)
                continue;

            // Find the matching URP -> HDRP shader pair for this material.
            string hdrpShaderName = null;

            foreach (var pair in ShaderMap)
            {
                if (mat.shader.name == pair.urpShader)
                {
                    hdrpShaderName = pair.hdrpShader;
                    break;
                }
            }

            if (hdrpShaderName == null)
                continue;

            Shader hdrpShader = Shader.Find(hdrpShaderName);

            if (hdrpShader == null)
            {
                Debug.LogError(
                    $"[HDRP Particle Converter] Shader '{hdrpShaderName}' not found. " +
                    $"Skipping '{mat.name}'."
                );
                continue;
            }

            Undo.RecordObject(mat, "Convert URP Particle Shader Advanced");

            // ============================================================
            // CACHE EVERYTHING FROM URP BEFORE CHANGING THE SHADER
            // ============================================================

            // URP particle texture
            Texture baseMap = null;

            if (mat.HasProperty("_BaseMap"))
            {
                baseMap = mat.GetTexture("_BaseMap");
            }
            else if (mat.HasProperty("_MainTex"))
            {
                baseMap = mat.GetTexture("_MainTex");
            }

            // Cache texture transform
            Vector2 textureScale = Vector2.one;
            Vector2 textureOffset = Vector2.zero;

            if (mat.HasProperty("_BaseMap"))
            {
                textureScale = mat.GetTextureScale("_BaseMap");
                textureOffset = mat.GetTextureOffset("_BaseMap");
            }
            else if (mat.HasProperty("_MainTex"))
            {
                textureScale = mat.GetTextureScale("_MainTex");
                textureOffset = mat.GetTextureOffset("_MainTex");
            }

            // Cache URP particle color
            Color baseColor = Color.white;

            if (mat.HasProperty("_BaseColor"))
            {
                baseColor = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color"))
            {
                baseColor = mat.GetColor("_Color");
            }

            // Normal
            Texture normalMap = null;

            if (mat.HasProperty("_BumpMap"))
            {
                normalMap = mat.GetTexture("_BumpMap");
            }

            // Emission
            Texture emissionMap = null;

            if (mat.HasProperty("_EmissionMap"))
            {
                emissionMap = mat.GetTexture("_EmissionMap");
            }

            Color emissionColor = Color.black;

            if (mat.HasProperty("_EmissionColor"))
            {
                emissionColor = mat.GetColor("_EmissionColor");
            }

            // Metallic / Smoothness (Lit particles only — absent on Unlit
            // materials, so these just stay at 0 for those and are unused)
            float metallic = 0f;
            float smoothness = 0.5f;
            bool hasMetallicSmoothness = mat.HasProperty("_Metallic") && mat.HasProperty("_Smoothness");

            if (hasMetallicSmoothness)
            {
                metallic = mat.GetFloat("_Metallic");
                smoothness = mat.GetFloat("_Smoothness");
            }

            // ============================================================
            // IMPORTANT:
            // Keep the original URP BaseMap reference before changing
            // the material shader.
            // ============================================================

            Texture cachedBaseMap = baseMap;

            // ============================================================
            // CHANGE SHADER
            // ============================================================

            mat.shader = hdrpShader;

            // ============================================================
            // HDRP BASE COLOR TEXTURE
            //
            // NOTE: HDRP/Unlit does NOT use "_BaseColorMap" / "_BaseColor".
            // Those property names belong to HDRP/Lit. HDRP/Unlit uses
            // "_UnlitColorMap" / "_UnlitColor" instead. That mismatch was
            // why the texture and tint were silently never applied
            // (HasProperty("_BaseColorMap") was always false on Unlit,
            // so it fell straight into the warning branch below).
            //
            // We check both naming conventions so this works correctly
            // for both the Unlit and Lit entries in ShaderMap above.
            // ============================================================

            string colorMapProp = mat.HasProperty("_UnlitColorMap")
                ? "_UnlitColorMap"
                : (mat.HasProperty("_BaseColorMap") ? "_BaseColorMap" : null);

            string colorProp = mat.HasProperty("_UnlitColor")
                ? "_UnlitColor"
                : (mat.HasProperty("_BaseColor") ? "_BaseColor" : null);

            if (cachedBaseMap != null)
            {
                if (colorMapProp != null)
                {
                    // URP _BaseMap -> HDRP _UnlitColorMap (or _BaseColorMap on Lit)
                    mat.SetTexture(colorMapProp, cachedBaseMap);
                    mat.SetTextureScale(colorMapProp, textureScale);
                    mat.SetTextureOffset(colorMapProp, textureOffset);
                }
                else
                {
                    Debug.LogWarning(
                        $"[HDRP Particle Converter] Material '{mat.name}' " +
                        "does not have a recognized HDRP base color map property."
                    );
                }
            }

            // ============================================================
            // HDRP BASE COLOR / TINT
            // ============================================================

            if (colorProp != null)
            {
                mat.SetColor(colorProp, baseColor);
            }

            // ============================================================
            // NORMAL MAP
            // ============================================================

            if (normalMap != null && mat.HasProperty("_NormalMap"))
            {
                mat.SetTexture("_NormalMap", normalMap);
                mat.EnableKeyword("_NORMALMAP");
            }

            // ============================================================
            // METALLIC / SMOOTHNESS (Lit particles only — values were
            // cached above, before the shader switch)
            // ============================================================

            if (hasMetallicSmoothness)
            {
                if (mat.HasProperty("_Metallic"))
                {
                    mat.SetFloat("_Metallic", metallic);
                }

                if (mat.HasProperty("_Smoothness"))
                {
                    mat.SetFloat("_Smoothness", smoothness);
                }
            }

            // ============================================================
            // EMISSION
            // ============================================================

            if (emissionMap != null ||
                emissionColor.r > 0 ||
                emissionColor.g > 0 ||
                emissionColor.b > 0)
            {
                if (mat.HasProperty("_EmissiveColorMap") &&
                    emissionMap != null)
                {
                    mat.SetTexture(
                        "_EmissiveColorMap",
                        emissionMap
                    );
                }

                if (mat.HasProperty("_EmissiveColor"))
                {
                    mat.SetColor(
                        "_EmissiveColor",
                        emissionColor
                    );
                }

                // Preserve HDR emission intensity
                float maxColorComponent = Mathf.Max(
                    emissionColor.r,
                    Mathf.Max(
                        emissionColor.g,
                        emissionColor.b
                    )
                );

                float emissionIntensity =
                    Mathf.Max(1.0f, maxColorComponent);

                if (mat.HasProperty("_EmissiveIntensity"))
                {
                    mat.SetFloat(
                        "_EmissiveIntensity",
                        emissionIntensity
                    );
                }

                mat.EnableKeyword("_EMISSION");
            }

            // ============================================================
            // TRANSPARENCY
            // ============================================================

            if (mat.HasProperty("_SurfaceType"))
            {
                // HDRP:
                // 0 = Opaque
                // 1 = Transparent
                mat.SetFloat("_SurfaceType", 1);
            }

            // URP particle blend mode
            float blendMode = 0;

            if (mat.HasProperty("_Blend"))
            {
                blendMode = mat.GetFloat("_Blend");
            }

            if (mat.HasProperty("_BlendMode"))
            {
                mat.SetFloat("_BlendMode", blendMode);
            }

            // ============================================================
            // RENDERING STATE
            // ============================================================

            mat.SetOverrideTag(
                "RenderType",
                "Transparent"
            );

            mat.SetInt(
                "_SrcBlend",
                (int)UnityEngine.Rendering.BlendMode.SrcAlpha
            );

            if (blendMode == 1)
            {
                // Additive
                mat.SetInt(
                    "_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.One
                );
            }
            else
            {
                // Alpha blend
                mat.SetInt(
                    "_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
                );
            }

            mat.SetInt("_ZWrite", 0);

            // ============================================================
            // SAVE
            // ============================================================

            EditorUtility.SetDirty(mat);
            count++;

            Debug.Log(
                $"[HDRP Particle Converter] Converted '{mat.name}' " +
                $"BaseMap: {(cachedBaseMap != null ? cachedBaseMap.name : "None")}"
            );
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"[HDRP Particle Converter] Successfully batch-converted " +
            $"{count} particle material(s)."
        );
    }
}