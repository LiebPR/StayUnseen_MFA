namespace MFA.Character
{
    #region Usings
    //---- C# --------------------------
    using System;
    //---- Unity -----------------------
    using UnityEngine;
    //---- MFA -------------------------
    using MFA.Core.FSM;
    using MFA.Inputs;
    #endregion

    /// <summary>
    /// Encapsula las referencias, dependencias y variables de estado compartidas del jugador.
    /// </summary>
    public class PlayerContext : IDisposable
    {
        #region References / Properties / Fields
        //---- Componentes -----------------
        /// <summary>
        /// Obtiene el componente <see cref="UnityEngine.Rigidbody"/> asociado al personaje para la simulación física.
        /// </summary>
        public Rigidbody Rigidbody { get; }

        //---- Systems ---------------------
        public StateMachine FSM { get; }
        public InputReader Input { get; }
        #endregion

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PlayerContext"/> y sus módulos de físicas asociados.
        /// </summary>
        public PlayerContext
        (
            Rigidbody rigidbody,
            StateMachine fsm,
            InputReader input
        )
        {
            Rigidbody = rigidbody;
            FSM = fsm;
            Input = input;
        }

        /// <summary>
        /// Libera los recursos administrados por el contexto, cancelando las suscripciones del lector de entradas.
        /// </summary>
        public void Dispose()
        {
            Input.Dispose();
        }
    }
}