namespace Game.Core.Interaction
{
    using MFA.Core.Interaction;
    #region Usings
    using System.Collections;
    using UnityEngine;
    #endregion

    public class Trapdoor : MonoBehaviour, IInteractable
    {
        [Header("Configuración de Apertura")]
        [Tooltip("Ángulo de rotación local cuando la trampilla está abierta.")]
        [SerializeField] private Vector3 openEulerAngle = new Vector3(-90f, 0f, 0f);

        [Header("Tiempos de Apertura")]
        [Tooltip("Tiempo de espera (en segundos) antes de comenzar a abrirse.")]
        [SerializeField] private float openDelay = 0f;
        [Tooltip("Duración en segundos de la animación al abrir la trampilla.")]
        [SerializeField] private float openDuration = 0.5f;

        [Header("Tiempos de Cierre")]
        [Tooltip("Tiempo de espera (en segundos) antes de comenzar a cerrarse.")]
        [SerializeField] private float closeDelay = 0f;
        [Tooltip("Duración en segundos de la animación al cerrar la trampilla.")]
        [SerializeField] private float closeDuration = 0.5f;

        [Header("Opcional")]
        [Tooltip("Si es true, la trampilla se puede cerrar volviendo a interactuar.")]
        [SerializeField] private bool isToggleable = true;

        // Rotaciones base
        private Quaternion closedRotation;
        private Quaternion openRotation;

        private bool isOpen = false;
        private bool isMoving = false;
        private Coroutine currentRoutine;

        private void Awake()
        {
            closedRotation = transform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openEulerAngle);
        }

        #region Implementación de IInteractable
        public void OnFocusGained()
        {
            Debug.Log($"[Trapdoor] Enfoque obtenido en {gameObject.name}");
        }

        public void OnFocusLost()
        {
            Debug.Log($"[Trapdoor] Enfoque perdido en {gameObject.name}");
        }

        public void OnInteractPressed()
        {
            if (isMoving) return; // Evita interrumpir la transición actual

            if (!isOpen)
            {
                Open();
            }
            else if (isToggleable)
            {
                Close();
            }
        }

        public void OnInteractReleased()
        {
            // No se requiere lógica al soltar para la trampilla
        }
        #endregion

        private void Open()
        {
            isOpen = true;
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(RotateTrapdoor(openRotation, openDelay, openDuration));
        }

        private void Close()
        {
            isOpen = false;
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(RotateTrapdoor(closedRotation, closeDelay, closeDuration));
        }

        private IEnumerator RotateTrapdoor(Quaternion targetRotation, float delay, float duration)
        {
            isMoving = true;

            // Esperar el tiempo de delay antes de iniciar la rotación
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            Quaternion startRotation = transform.localRotation;
            float elapsedTime = 0f;
            float safeDuration = Mathf.Max(duration, 0.0001f);

            while (elapsedTime < safeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / safeDuration);

                // Suavizado cúbico (SmoothStep) para un movimiento natural
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            transform.localRotation = targetRotation;
            isMoving = false;
        }
    }
}