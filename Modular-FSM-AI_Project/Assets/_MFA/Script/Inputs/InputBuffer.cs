namespace MFA.Inputs
{
    #region Usings
    using UnityEngine;
    #endregion

    /// <summary>
    /// Gestiona el almacenamiento temporal (buffer) de una entrada basada en el tiempo,
    /// permitiendo registrar pulsaciones ligeramente anticipadas sin requerir un bucle Update().
    /// </summary>
    public class InputBuffer
    {
        readonly float _bufferWindow;
        float _lastPressTime = -1f;

        /// <summary>
        /// Inicializa el buffer con una ventana de tiempo configurable (en segundos).
        /// </summary>
        public InputBuffer(float bufferWindow = 0.15f)
        {
            _bufferWindow = bufferWindow;
        }

        /// <summary>
        /// Registra una nueva pulsación del botón.
        /// </summary>
        public void Trigger()
        {
            _lastPressTime = Time.time;
        }

        /// <summary>
        /// Comprueba si hay una entrada válida en el buffer y, de ser así, la consume.
        /// </summary>
        /// <returns><c>true</c> si el buffer estaba activo dentro de la ventana de tiempo; de lo contrario, <c>false</c>.</returns>
        public bool Consume()
        {
            if (Time.time - _lastPressTime <= _bufferWindow)
            {
                _lastPressTime = -1f; // Invalida el buffer tras consumirlo
                return true;
            }
            return false;
        }
    }
}
