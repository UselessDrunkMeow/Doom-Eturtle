#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DoomEturtle.EditorTools
{
    /// <summary>
    /// Bulk GPU-instancing setup for the whole project.
    ///
    /// Menu: Tools > Rendering > GPU Instancing
    ///   * Report                      - read-only scan, changes nothing.
    ///   * Enable On All Materials     - flips "Enable GPU Instancing" on every .mat asset.
    ///   * Patch Shaders (advanced)    - injects "#pragma multi_compile_instancing" into
    ///                                   hand-written .shader files that lack it.
    ///
    /// Put this file in an "Editor" folder (it already is) so it never ships in a build.
    /// </summary>
    public static class GpuInstancingSetup
    {
        const string MenuRoot = "Tools/Rendering/GPU Instancing/";
        static readonly string[] SearchRoots = { "Assets" };

        // ShaderUtil.HasInstancing is not in the public docs and moved between Unity
        // versions, so reach it by reflection and fall back to "assume supported".
        static readonly MethodInfo HasInstancingMethod = typeof(ShaderUtil).GetMethod(
            "HasInstancing",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(Shader) },
            null);

        // ---------------------------------------------------------------- report

        [MenuItem(MenuRoot + "Report (no changes)", false, 0)]
        public static void Report()
        {
            var mats = LoadAllMaterials(out var skippedEmbedded);
            var alreadyOn = new List<Material>();
            var canEnable = new List<Material>();
            var unsupported = new List<Material>();

            foreach (var mat in mats)
            {
                if (!ShaderSupportsInstancing(mat.shader)) unsupported.Add(mat);
                else if (mat.enableInstancing) alreadyOn.Add(mat);
                else canEnable.Add(mat);
            }

            var sb = new StringBuilder();
            sb.AppendLine("=== GPU Instancing report ===");
            sb.AppendLine($"Materials scanned      : {mats.Count}");
            sb.AppendLine($"  already instanced    : {alreadyOn.Count}");
            sb.AppendLine($"  can be enabled       : {canEnable.Count}");
            sb.AppendLine($"  shader has no support: {unsupported.Count}");
            sb.AppendLine($"  embedded in models   : {skippedEmbedded} (read-only, skipped)");

            if (canEnable.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("-- Will be enabled --");
                foreach (var m in canEnable) sb.AppendLine($"   {AssetDatabase.GetAssetPath(m)}");
            }

            if (unsupported.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("-- Shader does not declare instancing support --");
                foreach (var g in unsupported.GroupBy(m => m.shader ? m.shader.name : "<missing shader>")
                                             .OrderByDescending(g => g.Count()))
                    sb.AppendLine($"   {g.Key}  ({g.Count()} material(s))");
            }

            var shaderIssues = ScanShaderSources();
            if (shaderIssues.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("-- Hand-written shaders missing the instancing pragma --");
                foreach (var s in shaderIssues) sb.AppendLine($"   {s.Path}{(s.HasInstanceMacros ? "" : "   (also missing UNITY_VERTEX_INPUT_INSTANCE_ID / UNITY_SETUP_INSTANCE_ID)")}");
            }

            var graphCount = AssetDatabase.FindAssets("t:Shader", SearchRoots)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Count(p => p.EndsWith(".shadergraph", StringComparison.OrdinalIgnoreCase));
            if (graphCount > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"-- {graphCount} Shader Graph asset(s) --");
                sb.AppendLine("   Shader Graph targets enable instancing by default. If one is off, toggle it in");
                sb.AppendLine("   Graph Inspector > Graph Settings; it is not safely scriptable, so it is left alone.");
            }

            Debug.Log(sb.ToString());
            EditorUtility.DisplayDialog(
                "GPU Instancing report",
                $"{mats.Count} materials scanned.\n\n" +
                $"{alreadyOn.Count} already instanced\n" +
                $"{canEnable.Count} can be enabled\n" +
                $"{unsupported.Count} on shaders without instancing support\n\n" +
                "Full breakdown written to the Console.",
                "OK");
        }

        // ---------------------------------------------------------------- materials

        [MenuItem(MenuRoot + "Enable On All Materials", false, 20)]
        public static void EnableOnAllMaterials()
        {
            var mats = LoadAllMaterials(out var skippedEmbedded);
            var targets = mats.Where(m => !m.enableInstancing && ShaderSupportsInstancing(m.shader)).ToList();
            var unsupported = mats.Count(m => !ShaderSupportsInstancing(m.shader));

            if (targets.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "GPU Instancing",
                    $"Nothing to change.\n\n{mats.Count} materials scanned, " +
                    $"{unsupported} of them use a shader without instancing support.",
                    "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                    "Enable GPU Instancing",
                    $"Enable GPU instancing on {targets.Count} material(s)?\n\n" +
                    $"Scanned: {mats.Count}\n" +
                    $"Skipped (shader has no support): {unsupported}\n" +
                    $"Skipped (embedded in a model): {skippedEmbedded}\n\n" +
                    "This is undoable (Ctrl/Cmd+Z) and the asset files are rewritten on save.",
                    "Enable", "Cancel"))
                return;

            Undo.SetCurrentGroupName("Enable GPU Instancing");
            int group = Undo.GetCurrentGroup();
            int changed = 0;

            try
            {
                AssetDatabase.StartAssetEditing();
                for (int i = 0; i < targets.Count; i++)
                {
                    var mat = targets[i];
                    if (EditorUtility.DisplayCancelableProgressBar(
                            "Enabling GPU Instancing",
                            AssetDatabase.GetAssetPath(mat),
                            (float)i / targets.Count))
                        break;

                    Undo.RecordObject(mat, "Enable GPU Instancing");
                    mat.enableInstancing = true;
                    EditorUtility.SetDirty(mat);
                    changed++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }

            Undo.CollapseUndoOperations(group);
            AssetDatabase.SaveAssets();

            Debug.Log($"[GPU Instancing] Enabled on {changed} material(s). " +
                      $"{unsupported} material(s) use a shader that does not support instancing — run the Report for the list.");
        }

        [MenuItem(MenuRoot + "Disable On All Materials", false, 21)]
        public static void DisableOnAllMaterials()
        {
            var mats = LoadAllMaterials(out _);
            var targets = mats.Where(m => m.enableInstancing).ToList();
            if (targets.Count == 0 ||
                !EditorUtility.DisplayDialog("Disable GPU Instancing",
                    $"Turn GPU instancing OFF on {targets.Count} material(s)?", "Disable", "Cancel"))
                return;

            Undo.SetCurrentGroupName("Disable GPU Instancing");
            int group = Undo.GetCurrentGroup();
            foreach (var mat in targets)
            {
                Undo.RecordObject(mat, "Disable GPU Instancing");
                mat.enableInstancing = false;
                EditorUtility.SetDirty(mat);
            }
            Undo.CollapseUndoOperations(group);
            AssetDatabase.SaveAssets();
            Debug.Log($"[GPU Instancing] Disabled on {targets.Count} material(s).");
        }

        // ---------------------------------------------------------------- shaders

        [MenuItem(MenuRoot + "Patch Shader Sources (advanced)", false, 40)]
        public static void PatchShaderSources()
        {
            var issues = ScanShaderSources();
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog("GPU Instancing",
                    "Every hand-written .shader in the project already declares instancing support.", "OK");
                return;
            }

            int needMacros = issues.Count(i => !i.HasInstanceMacros);
            if (!EditorUtility.DisplayDialog(
                    "Patch shader sources",
                    $"Add \"#pragma multi_compile_instancing\" to {issues.Count} shader file(s)?\n\n" +
                    $"WARNING: the pragma alone only makes the checkbox appear. {needMacros} of these shaders " +
                    "do not use UNITY_VERTEX_INPUT_INSTANCE_ID / UNITY_SETUP_INSTANCE_ID, and without those macros " +
                    "instanced objects will all draw at the same position.\n\n" +
                    "A .bak copy of each file is written next to it. Review the diffs afterwards.",
                    "Patch", "Cancel"))
                return;

            int patched = 0;
            foreach (var issue in issues)
            {
                var full = Path.GetFullPath(issue.Path);
                var text = File.ReadAllText(full);

                // Insert the pragma right after each fragment pragma, keeping the indentation.
                var patchedText = Regex.Replace(
                    text,
                    @"(?m)^([ \t]*)#pragma[ \t]+fragment[ \t]+\w+[^\r\n]*$",
                    m => m.Value + Environment.NewLine + m.Groups[1].Value + "#pragma multi_compile_instancing");

                if (patchedText == text)
                {
                    Debug.LogWarning($"[GPU Instancing] No \"#pragma fragment\" found in {issue.Path} — patch it by hand.");
                    continue;
                }

                File.Copy(full, full + ".bak", true);
                File.WriteAllText(full, patchedText);
                patched++;

                if (!issue.HasInstanceMacros)
                    Debug.LogWarning($"[GPU Instancing] {issue.Path} still needs UNITY_VERTEX_INPUT_INSTANCE_ID in its " +
                                     "appdata/v2f structs and UNITY_SETUP_INSTANCE_ID(v) in the vertex function.");
            }

            AssetDatabase.Refresh();
            Debug.Log($"[GPU Instancing] Patched {patched} shader file(s). Backups saved as *.shader.bak.");
        }

        // ---------------------------------------------------------------- helpers

        struct ShaderIssue
        {
            public string Path;
            public bool HasInstanceMacros;
        }

        static List<Material> LoadAllMaterials(out int skippedEmbedded)
        {
            skippedEmbedded = 0;
            var result = new List<Material>();

            foreach (var guid in AssetDatabase.FindAssets("t:Material", SearchRoots))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Materials that live inside an .fbx/.blend are read-only.
                if (!path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                {
                    skippedEmbedded++;
                    continue;
                }

                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;
                if ((mat.hideFlags & HideFlags.NotEditable) != 0) { skippedEmbedded++; continue; }

                result.Add(mat);
            }

            return result;
        }

        static bool ShaderSupportsInstancing(Shader shader)
        {
            if (shader == null) return false;
            if (HasInstancingMethod == null) return true; // unknown Unity version: don't block the user
            try { return (bool)HasInstancingMethod.Invoke(null, new object[] { shader }); }
            catch { return true; }
        }

        static List<ShaderIssue> ScanShaderSources()
        {
            var issues = new List<ShaderIssue>();

            foreach (var guid in AssetDatabase.FindAssets("t:Shader", SearchRoots))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".shader", StringComparison.OrdinalIgnoreCase)) continue; // .shadergraph, .compute, ...

                string text;
                try { text = File.ReadAllText(Path.GetFullPath(path)); }
                catch { continue; }

                if (text.Contains("multi_compile_instancing") ||
                    text.Contains("instancing_options") ||
                    text.Contains("#pragma surface"))   // surface shaders get instancing variants for free
                    continue;

                issues.Add(new ShaderIssue
                {
                    Path = path,
                    HasInstanceMacros = text.Contains("UNITY_VERTEX_INPUT_INSTANCE_ID")
                });
            }

            return issues;
        }
    }
}
#endif
