namespace MFA.Core.FSM
{
    /// <summary>
    /// Contrato base para la definición de estados dentro de una máquina de estados finita (FSM).
    /// </summary>
    /// <remarks>
    /// Diseñado para encapsular y aislar la lógica de comportamientos específicos de las entidades.
    /// Las implementaciones concretas deben evitar asignaciones de memoria en el Heap (GC Allocations) dentro de <see cref="Tick"/> y <see cref="FixedTick"/>.
    /// </remarks>
    public interface IState
    {
        /// <summary>
        /// Invocado una sola vez al ingresar a este estado.
        /// </summary>
        /// <remarks>
        /// Utilizado para la inicialización de recursos localizados, suscripción a eventos o activación de animaciones iniciales.
        /// </remarks>
        void Enter();

        /// <summary>
        /// Invocado de forma recurrente en cada cuadro dentro del ciclo de actualización lógica.
        /// </summary>
        /// <remarks>
        /// Corresponde al ciclo <c>Update</c> de Unity. Debe ejecutarse exclusivamente en el hilo principal (Main Thread).
        /// </remarks>
        void Tick();

        /// <summary>
        /// Invocado en intervalos de tiempo fijos sincronizados con la simulación del motor de física.
        /// </summary>
        /// <remarks>
        /// Corresponde al ciclo <c>FixedUpdate</c> de Unity. Toda manipulación de <c>Rigidbody</c> o fuerzas físicas debe realizarse aquí.
        /// </remarks>
        void FixedTick();

        /// <summary>
        /// Invocado una sola vez inmediatamente antes de abandonar este estado.
        /// </summary>
        /// <remarks>
        /// Utilizado para la limpieza de recursos temporales, desuscripción de eventos y detención de comportamientos activos.
        /// </remarks>
        void Exit();
    }
}