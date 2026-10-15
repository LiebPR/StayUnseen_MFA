namespace MFA.Character
{
    #region Usings
    //---- C# --------------------------
    using System;
    //---- Unity -----------------------
    using UnityEngine;
    //---- MFA -------------------------
    using MFA.Core.FSM;
    using MFA.Core.Physics;
    using MFA.Core.Animations;
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
        public Transform CameraTransform { get; }
        public Animator Animator { get; }

        //---- Systems ---------------------
        public StateMachine FSM { get; }
        public InputReader Input { get; }

        public PhysicsSystem Physics {  get; }

        //---- Settings ------------------------------
        public CharacterSO Settings { get; }

        //---- Instancias Unicas Estados -------------
        public IState IdleState { get; set; }
        public IState MoveState { get; set; }
        public IState RunState { get; set; }
        #endregion

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PlayerContext"/> y sus módulos de físicas asociados.
        /// </summary>
        public PlayerContext
        (
            Rigidbody rigidbody,
            Transform cameraTransform,
            Animator animator,
            CharacterSO settings,
            StateMachine fsm,
            InputReader input,
            PhysicsSystem physics
        )
        {
            Rigidbody = rigidbody;
            CameraTransform = cameraTransform;
            Animator = animator;
            Settings = settings;
            FSM = fsm;
            Input = input;
            Physics = physics;
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