using UnityEngine;
using UnityEditor;
using System.IO;

public class BulkRenamerWindow : EditorWindow
{
    public enum PrefixType
    {
        None, 
        SM_,   //Static Mesh
        AN_,   //Animation
        AC_,   //Animator Controller
        PF_,   //Prefab
        VFX_,  //Visual Effects
        SFX_,  //Sound Effects
        MAT_,  //Materials
        T_,    //Textures
        Custom
    }

    public enum CasingStyle
    {
        CapitalizeWords, //material_tipo2 -> Material_Tipo2
        Lowercase,       //material_tipo2 -> material_tipo2
        Uppercase        //material_tipo2 -> MATERIAL_TIPO2
    }

    PrefixType selectedPrefix = PrefixType.None;
    string customPrefix = "";
    CasingStyle selectedCasing = CasingStyle.CapitalizeWords;

    [MenuItem("Tools/Bulk Renamer")]
    public static void ShowWindow()
    {
        GetWindow<BulkRenamerWindow>("Bulk Rename");
    }

    void OnGUI()
    {
        GUILayout.Label("Renamer Config", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        #region Selección Prefijo
        selectedPrefix = (PrefixType)EditorGUILayout.EnumPopup("Prefix:", selectedPrefix);
        if (selectedPrefix == PrefixType.Custom)
        {
            customPrefix = EditorGUILayout.TextField("Custom Prefix:", customPrefix);
        }
        #endregion

        #region Selección de Estilo de Texto
        selectedCasing = (CasingStyle)EditorGUILayout.EnumPopup("Text Style:", selectedCasing);

        EditorGUILayout.Space(10);
        #endregion

        #region Información de selección y Vista Previa
        int selectedCount = Selection.objects.Length;
        EditorGUILayout.HelpBox($"Selected folders in Project: {selectedCount}", MessageType.Info);

        if (selectedCount > 0)
        {
            Object sampleAsset = Selection.objects[0];
            string samplePath = AssetDatabase.GetAssetPath(sampleAsset);

            if (!Directory.Exists(samplePath))
            {
                string sampleOld = Path.GetFileNameWithoutExtension(samplePath);
                string sampleNew = GenerateNewName(samplePath, GetCurrentPrefix(), selectedCasing);

                EditorGUILayout.LabelField($"PreView(first element):", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"{sampleOld}  ➜  {sampleNew}", EditorStyles.wordWrappedLabel);
            }
        }

        EditorGUILayout.Space(15);
        #endregion

        #region Botón de Ejecución
        EditorGUI.BeginDisabledGroup(selectedCount == 0);
        if (GUILayout.Button("Selected Rename", GUILayout.Height(35)))
        {
            RenameSelectedAssets();
        }
        EditorGUI.EndDisabledGroup();
        #endregion
    }

    string GetCurrentPrefix()
    {
        if (selectedPrefix == PrefixType.None) return "";
        if (selectedPrefix == PrefixType.Custom) return customPrefix;
        return selectedPrefix.ToString();
    }

    void RenameSelectedAssets()
    {
        Object[] selectedAssets = Selection.objects;
        string prefix = GetCurrentPrefix();
        int renamedCount = 0;

        //Optimiza el rentimiento bloqueado reimportaciones individuales durante el bucle.
        AssetDatabase.StartAssetEditing();

        try
        {
            foreach (Object assets in selectedAssets)
            {
                string path = AssetDatabase.GetAssetPath(assets);

                //Omitir si es una carpeta o una ruta inválida
                if (string.IsNullOrEmpty(path) || Directory.Exists(path)) continue;

                string oldName = Path.GetFileNameWithoutExtension(path);
                string newName = GenerateNewName(oldName, prefix, selectedCasing);

                if (oldName == newName) continue;

                string error = AssetDatabase.RenameAsset(path, newName);

                if (string.IsNullOrEmpty(error))
                {
                    renamedCount++;
                }
                else
                {
                    Debug.LogError($"Renaming Error '{oldName}': {error}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[Bulk Rename] ¡Procces Completed! Renamed {renamedCount} / {selectedAssets.Length} files.");
    }

    string GenerateNewName(string originalName, string prefix, CasingStyle casing)
    {
        //Limpia el prefijo previo si ya lo tiene para evitar duplicados (ej: MAT_MAT_...)
        if (!string.IsNullOrEmpty(prefix) && originalName.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
        {
            originalName = originalName.Substring(prefix.Length);
        }

        //Aplica el formato de casing solicitado
        string processedName = originalName;

        switch (casing)
        {
            case CasingStyle.CapitalizeWords:
                processedName = CapitalizeWords(originalName);
                break;

            case CasingStyle.Lowercase:
                processedName = originalName.ToLower();
                break;

            case CasingStyle.Uppercase:
                processedName = originalName.ToUpper();
                break;
        }

        //Conecta el prefijo final
        return prefix + processedName;
    }

    private string CapitalizeWords(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Separa por guiones bajos para capitalizar cada segmento manteniendo la estructura
        string[] parts = input.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
            {
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
            }
        }
        return string.Join("_", parts);
    }
}
