#region Usings
//---- C# ---------
using System.IO;
//---- Unity ------
using UnityEditor;
using UnityEngine;
#endregion

/// <summary>
/// Herramienta de automatizción para la generación masiva de Prefabs en el Editor de Unity.
/// 
/// <b>Reglas y Convenciones:</b>
/// <list type="bullet">
/// <item><description>Aplica la regla de nomenclatura del proyecto cambiando el prefijo <c>SM_</c> (Static Mesh) por <c>PF_</c> (Prefab).</description></item>
/// <item><description>Garantiza que los assets se guarden siempre dentro de la estructura de carpetas oficial (<c>Assets/...</c>).</description></item>
/// </list>
/// </summary>
public static class PrefabAutomator
{
    /// <summary>
    /// Ejecuta el proceso principal de conversión: solicita una carpeta de destino al usuario
    /// y procesa cada objeto seleccionado para transformarlo en un Prefab.
    /// </summary>
    /// <remarks>
    /// <b>Ubicación en el Menú:</b> Accesible desde la barra superior en <c>Tools > Prefab Automator</c>.
    /// </remarks>
    [MenuItem("Tools/Prefab Automator", false, 2000)]
    public static void GeneratePrefabsFromSelection()
    {
        #region--- Obtención De Selección ---
        //Recuperación de los elementos actualmente marcados en la ventana Project o Hierarchy.
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("[PrefabAutomator] No hay assets seleccionados.");
            return;
        }
        #endregion

        #region--- Configuración De Destino ---
        //Solicitud de elección de la carpeta donde se organizarán los nuevos Prefabs.
        string absoluteFolderPath = EditorUtility.OpenFolderPanel("Seleccionar carpeta para guardar los Prefabs", "Assets", "");

        if (string.IsNullOrEmpty(absoluteFolderPath))
        {
            //Se a cancelado la ventana de selección de carpeta.
            return;
        }

        //Validación de la carpeta seleccionada esté dentro de la estructura del proyecto.
        string relativeFolderPath = ConvertToRelativePath(absoluteFolderPath);

        if (string.IsNullOrEmpty(relativeFolderPath))
        {
            Debug.LogError("[PrefabAutomator] La carpeta de destino debe estar dentro del directorio 'Assets' del proyecto.");
            return;
        }
        #endregion

        #region--- Procesamiento Batch (Conversión) ---
        int prefabsCreated = 0;

        foreach (Object obj in selectedObjects)
        {
            Mesh mesh = obj as Mesh;
            GameObject modelAsset = obj as GameObject;

            //Ignoramos assets que no sean mallas ni modelos 3D
            if (mesh == null && modelAsset == null) continue;

            //Determinar el nombre estandarizado del nuevo Prefab
            string originalName = obj.name;
            string prefabName = ConvertToPFName(originalName);

            //Crear el contenedor temporal para estructurar el Prefab
            GameObject rootGO = new GameObject(prefabName);

            //Casos de construcción según el tipo de asset origen
            if (mesh != null)
            {
                // Caso A: Asset de malla individual (Mesh) -> Asignamos componentes visuales básicos
                MeshFilter filter = rootGO.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;
                rootGO.AddComponent<MeshRenderer>();
            }
            else if (modelAsset != null)
            {
                // Caso B: Modelo 3D jerárquico (FBX/OBJ) -> Instanciamos la jerarquía completa
                GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
                modelInstance.transform.SetParent(rootGO.transform, false);
            }
        #endregion

        #region--- Guardado Y Registro En Proyecto ---
            string fullPath = Path.Combine(relativeFolderPath, $"{prefabName}.prefab");
            //Evita sobrescribir assets existentes generando nombres únicos si hay duplicados
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            //Guardar la estructura creada como un nuevo Prefab Asset
            PrefabUtility.SaveAsPrefabAsset(rootGO, fullPath);

            //Limpieza del objeto temporal en memoria
            Object.DestroyImmediate(rootGO);
            prefabsCreated++;
            
        }
        #endregion

        #region--- Finalización y Refrescado ---
        AssetDatabase.Refresh();
        Debug.Log($"[PrefabAutomator] Se han creado {prefabsCreated} prefabs en '{relativeFolderPath}'.");
        #endregion
    }

    /// <summary>
    /// Valida si la opción del menú contextual debe estar activa según el tipo de objeto seleccionado.
    /// </summary>
    /// <returns>
    /// <c>true</c> si hay al menos una malla o modelo seleccionado; de lo contrario, <c>false</c>.
    /// </returns>
    [MenuItem("Tools/Prefab Automator", true)]
    private static bool ValidateGeneratePrefabs()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is Mesh || obj is GameObject) return true;
        }
        return false;
    }

    /// <summary>
    /// Convierte el nombre del asset original según las convenciones de nomenclatura del proyecto.
    /// </summary>
    /// <param name="rawName">Nombre original del objeto (ej: "SM_Silla_01").</param>
    /// <returns>Nombre procesado con el prefijo estandarizado (ej: "PF_Silla_01").</returns>
    private static string ConvertToPFName(string rawName)
    {
        if (rawName.StartsWith("SM_"))
        {
            return "PF_" + rawName.Substring(3);
        }
        return "PF_" + rawName;
    }

    /// <summary>
    /// Transforma una ruta absoluta de disco en una ruta relativa compatible con el AssetDatabase de Unity.
    /// </summary>
    /// <param name="absolutePath">Ruta absoluta del sistema de archivos.</param>
    /// <returns>Ruta relativa que inicia con "Assets/..." o <c>null</c> si está fuera del proyecto.</returns>
    private static string ConvertToRelativePath(string absolutePath)
    {
        string projectPath = Application.dataPath;

        //Normalización de separadores de ruta (Windows / Mac)
        absolutePath = absolutePath.Replace('\\', '/');
        projectPath = projectPath.Replace('\\', '/');

        if (absolutePath.StartsWith(projectPath))
        {
            return "Assets" + absolutePath.Substring(projectPath.Length);
        }

        return null;
    }
}