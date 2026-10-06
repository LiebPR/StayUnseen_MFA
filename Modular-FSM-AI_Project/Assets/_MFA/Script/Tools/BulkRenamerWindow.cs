#region Usings
//---- C# ---------
using System.IO;
//---- Unity ------
using UnityEngine;
using UnityEditor;
#endregion

/// <summary>
/// Ventana personalizada del Editor de Unity para realizar renombrado masivo de assets en el pryecto.
/// Permite añadir prefijos estandarizados y modificar el estilo de capitalización (Casing).
/// 
/// /// <b>Garantías de Integridad:</b> Congela las reimportaciones individuales del pipeline mediante <see cref="AssetDatabase.StartAssetEditing"/>
/// y garantiza su reanudación con <see cref="AssetDatabase.StopAssetEditing"/> dentro de un bloque <c>try-finally</c>.
/// </summary>
public class BulkRenamerWindow : EditorWindow
{
    #region Enumeradores
    /// <summary>
    /// Define los tipos de prefijo estandarizados para categorizar los assets del proyecto.
    /// </summary>
    public enum PrefixType
    {
        /// <summary>Sin prefijo adicional.</summary>
        None,
        /// <summary>Malla estática (Static Mesh).</summary>
        SM_,
        /// <summary>Clip de animación (Animation Clip).</summary>
        AN_,
        /// <summary>Controlador de animación (Animator Controller).</summary>
        AC_,
        /// <summary>Asset prefabricado (Prefab).</summary>
        PF_,
        /// <summary>Efecto visual (VFX / Particle System / Visual Effect Graph).</summary>
        VFX_,
        /// <summary>Efecto de sonido (Sound Effect / Audio Clip).</summary>
        SFX_,
        /// <summary>Material de renderizado.</summary>
        MAT_,
        /// <summary>Textura o mapa de imagen.</summary>
        T_,
        /// <summary>Prefijo personalizado especificado manualmente por el usuario.</summary>
        Custom
    }

    /// <summary>
    /// Define las reglas de formato de texto o capitalización aplicables a los nombres de los assets.
    /// </summary>
    public enum CasingStyle
    {
        /// <summary>Capitaliza la primera letra de cada palabra separada por guion bajo (ej. <c>material_tipo2</c> -> <c>Material_Tipo2</c>).</summary>
        CapitalizeWords,
        /// <summary>Transforma la cadena completamente a minúsculas.</summary>
        Lowercase,
        /// <summary>Transforma la cadena completamente a mayúsculas.</summary>
        Uppercase
    }
    #endregion

    #region Serializable Fields
    [Header("Configuración de Renombrado")]
    [SerializeField, Tooltip("Tipo de prefijo que se antepondrá a los archivos seleccionados.")]
    private PrefixType selectedPrefix = PrefixType.None;

    [SerializeField, Tooltip("Cadena de prefijo personalizada. Requerida únicamente cuando 'selectedPrefix' es 'Custom'.")]
    private string customPrefix = "";

    [SerializeField, Tooltip("Regla de capitalización aplicable al nombre base de cada asset.")]
    private CasingStyle selectedCasing = CasingStyle.CapitalizeWords;
    #endregion

    /// <summary>
    /// Abre o enfoca la ventana de la herramienta en la interfaz del Editor de Unity.
    /// </summary>
    /// <remarks>
    /// Registra la entrada del menú bajo el menú superior <c>Tools/Bulk Renamer</c>.
    /// </remarks>
    [MenuItem("Tools/Bulk Renamer")]
    public static void ShowWindow()
    {
        GetWindow<BulkRenamerWindow>("Bulk Rename");
    }

    /// <summary>
    /// Renderiza y procesa los eventos de la interfaz de usuario mediante la API de IMGUI.
    /// </summary>
    /// <remarks>
    /// <b>Ciclo de Vida y Frecuencia:</b> Invocado múltiples veces por segundo en respuesta a eventos de entrada o repintado del Editor.<br/>
    /// <b>Asignaciones de Memoria (GC):</b> Produce asignaciones temporales en el Heap por la construcción continua de layouts de GUI y concatenación de cadenas en la vista previa.
    /// </remarks>
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

    /// <summary>
    /// Evalúa la opción de prefijo seleccionada y retorna su valor equivalente en texto.
    /// </summary>
    /// <returns>Cadena de texto con el prefijo formateado o <see cref="string.Empty"/> si el valor es <see cref="PrefixType.None"/>.</returns>
    string GetCurrentPrefix()
    {
        if (selectedPrefix == PrefixType.None) return "";
        if (selectedPrefix == PrefixType.Custom) return customPrefix;
        return selectedPrefix.ToString();
    }

    /// <summary>
    /// Ejecuta el proceso de renombrado en lote sobre los assets actualmente seleccionados en la ventana Project.
    /// </summary>
    /// <remarks>
    /// <b>Rendimiento y Pipeline de Assets:</b> Usa <see cref="AssetDatabase.StartAssetEditing"/> para bloquear las reimportaciones individuales 
    /// en disco durante la iteración.<br/>
    /// El bloque <c>finally</c> garantiza la ejecución de <see cref="AssetDatabase.StopAssetEditing"/> ante cualquier excepción no controlada, evitando bloquear la base de datos de assets.
    /// </remarks>
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

    /// <summary>
    /// Aplica las modificaciones de prefijo y capitalización al nombre original de un asset.
    /// </summary>
    /// <param name="originalName">El nombre base original del archivo (sin ruta ni extensión). No debe ser nulo.</param>
    /// <param name="prefix">Cadena del prefijo que se antepondrá. Admite cadena vacía o nula.</param>
    /// <param name="casing">Estilo de formato de texto o capitalización a aplicar.</param>
    /// <returns>El nuevo nombre procesado y listo para asignarse en el sistema de archivos de Unity.</returns>
    /// <remarks>
    /// Previene la duplicación no deseada de prefijos mediante una comprobación previa insensible a mayúsculas y minúsculas (<see cref="System.StringComparison.OrdinalIgnoreCase"/>).
    /// </remarks>
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

    /// <summary>
    /// Formatea una cadena compuesta por palabras separadas por guion bajo, capitalizando el primer carácter de cada palabra.
    /// </summary>
    /// <param name="input">Cadena de texto fuente a transformar. Si es nula o vacía, se retorna intacta.</param>
    /// <returns>La cadena formateada en estilo capitalizado o la cadena original si es inválida.</returns>
    /// <remarks>
    /// Operación de procesamiento de cadenas que genera asignaciones temporales en el Heap debido a las llamadas a <see cref="string.Split(char[])"/> y <see cref="string.Join(string, string[])"/>.
    /// </remarks>
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
