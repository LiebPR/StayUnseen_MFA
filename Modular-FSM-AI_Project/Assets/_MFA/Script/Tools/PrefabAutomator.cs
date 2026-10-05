using System.IO;
using UnityEditor;
using UnityEngine;

public static class PrefabAutomator
{
    [MenuItem("Tools/Prefab Automator", false, 2000)]
    public static void GeneratePrefabsFromSelection()
    {
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("[PrefabAutomator] No hay assets seleccionados.");
            return;
        }

        // 1. Abrir diálogo para elegir carpeta de destino
        string absoluteFolderPath = EditorUtility.OpenFolderPanel("Seleccionar carpeta para guardar los Prefabs", "Assets", "");

        // Si el usuario cancela la selección
        if (string.IsNullOrEmpty(absoluteFolderPath))
        {
            return;
        }

        // 2. Convertir la ruta absoluta a una ruta relativa de Unity (Assets/...)
        string relativeFolderPath = ConvertToRelativePath(absoluteFolderPath);

        if (string.IsNullOrEmpty(relativeFolderPath))
        {
            Debug.LogError("[PrefabAutomator] La carpeta de destino debe estar dentro del directorio 'Assets' del proyecto.");
            return;
        }

        int prefabsCreated = 0;

        foreach (Object obj in selectedObjects)
        {
            Mesh mesh = obj as Mesh;
            GameObject modelAsset = obj as GameObject;

            if (mesh == null && modelAsset == null) continue;

            string originalName = obj.name;
            string prefabName = ConvertToPFName(originalName);

            // Crear GameObject padre en memoria
            GameObject rootGO = new GameObject(prefabName);

            if (mesh != null)
            {
                MeshFilter filter = rootGO.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;
                rootGO.AddComponent<MeshRenderer>();
            }
            else if (modelAsset != null)
            {
                GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
                modelInstance.transform.SetParent(rootGO.transform, false);
            }

            // Generar la ruta final dentro de la carpeta elegida
            string fullPath = Path.Combine(relativeFolderPath, $"{prefabName}.prefab");
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            PrefabUtility.SaveAsPrefabAsset(rootGO, fullPath);

            Object.DestroyImmediate(rootGO);
            prefabsCreated++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[PrefabAutomator] Se han creado {prefabsCreated} prefabs en '{relativeFolderPath}'.");
    }

    [MenuItem("Assets/Create/Generar Prefab desde SM (PF_)", true)]
    private static bool ValidateGeneratePrefabs()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is Mesh || obj is GameObject) return true;
        }
        return false;
    }

    private static string ConvertToPFName(string rawName)
    {
        if (rawName.StartsWith("SM_"))
        {
            return "PF_" + rawName.Substring(3);
        }
        return "PF_" + rawName;
    }

    private static string ConvertToRelativePath(string absolutePath)
    {
        string projectPath = Application.dataPath; // Devuelve 'C:/Ruta/De/TuProyecto/Assets'

        // Normalizar separadores de directorio a barras inclinadas '/'
        absolutePath = absolutePath.Replace('\\', '/');
        projectPath = projectPath.Replace('\\', '/');

        if (absolutePath.StartsWith(projectPath))
        {
            return "Assets" + absolutePath.Substring(projectPath.Length);
        }

        return null; // La carpeta elegida está fuera de Assets
    }
}