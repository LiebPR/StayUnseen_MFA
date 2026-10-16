namespace MFA.Core.Interaction
{
    #region Usings
    //---- Unity ------
    using UnityEngine;
    #endregion

    /// <summary>
    /// Configuración de variables y datos estadísticos para el sistema de interacción.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_Name_InteractionConfig", menuName = "Game/Core/Interaction Settings")]
    public class InteractionSO : ScriptableObject
    {
        [Header("Configuración de Caja de Detección")]
        [Tooltip("Dimensiones totales de la caja (Ancho, Alto, Profundidad).")]
        [SerializeField] private Vector3 boxSize = new Vector3(1.5f, 2f, 1.5f);

        [Tooltip("Offset relativo al transform del personaje para posicionar el centro de la caja.")]
        [SerializeField] private Vector3 boxOffset = new Vector3(0f, 1f, 1f);

        [Tooltip("Capa (LayerMask) en la que se encuentran los objetos interactuables.")]
        [SerializeField] private LayerMask interactableLayer;

        [Tooltip("Tamaño máximo del buffer de colisionadores para la detección en caja.")]
        [SerializeField] private int maxColliders = 5;

        #region Getters
        public Vector3 BoxSize => boxSize;
        public Vector3 BoxOffset => boxOffset;
        public Vector3 HalfExtents => boxSize * 0.5f;
        public LayerMask InteractableLayer => interactableLayer;
        public int MaxColliders => maxColliders;
        #endregion
    }
}