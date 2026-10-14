namespace MFA.Core.FSM
{
    /// <summary>
    /// Orquesta las transiciones y la ejecución del ciclo de vida de los estados que implementan <see cref="IState"/>.
    /// </summary>
    /// <remarks>
    /// Implementa el patrón de diseño Finite State Machine (FSM) de forma desacoplada del ciclo de vida de <c>MonoBehaviour</c>.
    /// Garantiza una ejecución sin asignaciones de memoria (Zero GC Allocations) en tiempo de ejecución durante los ciclos de actualización.
    /// </remarks>
    public class StateMachine
    {
        #region Properties & Fields
        private IState _currentState;

        /// <summary>
        /// Obtiene la referencia al estado activo actual dentro de la máquina de estados.
        /// </summary>
        public IState CurrentState => _currentState;
        #endregion

        #region Public Methods (API)
        /// <summary>
        /// Transiciona la máquina hacia un nuevo estado realizando la secuencia de salida e ingreso correspondiente.
        /// </summary>
        /// <param name="newState">Instancia del nuevo estado que tomará el control. Admite valores nulos para desactivar la ejecución.</param>
        /// <remarks>
        /// Ejecuta <see cref="IState.Exit"/> en el estado actual (si existe) antes de actualizar la referencia e invocar <see cref="IState.Enter"/> en el nuevo estado.
        /// </remarks>
        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        /// <summary>
        /// Avanza la lógica por cuadro del estado activo.
        /// </summary>
        /// <remarks>
        /// Debe ser invocado regularmente desde el método <c>Update</c> de un componente controlador o a través de un despachador de ciclo de vida (<see cref="ITickable"/>).
        /// </remarks>
        public void Update()
        {
            _currentState?.Tick();
        }

        /// <summary>
        /// Avanza la lógica del paso de física del estado activo.
        /// </summary>
        /// <remarks>
        /// Debe ser invocado regularmente desde el método <c>FixedUpdate</c> de un componente controlador o un sistema de actualización fija (<see cref="IFixedTickable"/>).
        /// </remarks>
        public void FixedUpdate()
        {
            _currentState?.FixedTick();
        }
        #endregion
    }
}