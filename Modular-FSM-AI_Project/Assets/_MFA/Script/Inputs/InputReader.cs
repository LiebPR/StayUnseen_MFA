namespace MFA.Inputs
{
    #region Usings
    //---- C# ---------------------
    using System;
    //---- Unity ------------------
    using UnityEngine;
    using UnityEngine.InputSystem;
    #endregion

    /// <summary>
    /// Encapsula y gestiona la lectura de entradas del jugador utilizando el New Input System de Unity
    /// adaptado al esquema de acciones: Movement, Run, Interact, Throw y Crouch.
    /// </summary>
    public class InputReader : PlayerInputActions.IGameplayActions, IDisposable
    {
        #region Fields & Dependencies
        readonly PlayerInputActions _inputActions;

        // Buffers para acciones que se benefician de respuesta tolerante
        public readonly InputBuffer InteractBuffer = new InputBuffer(0.15f);
        public readonly InputBuffer ThrowBuffer = new InputBuffer(0.15f);
        #endregion

        #region Properties
        //---- Movimiento ---------------------------------
        /// <summary>
        /// Obtiene el vector bidimensional de movimiento actual.
        /// </summary>
        public Vector2 MoveDirection { get; private set; }

        //---- Estados Continuos ---------------------------
        /// <summary>
        /// Indica si el botón de correr se mantiene presionado.
        /// </summary>
        public bool IsRunPressed { get; private set; }

        /// <summary>
        /// Indica si el botón de agacharse se mantiene presionado.
        /// </summary>
        public bool IsCrouchPressed { get; private set; }

        /// <summary>
        /// Indica si el botón de interacción se mantiene presionado.
        /// </summary>
        public bool IsInteractPressed { get; private set; }

        /// <summary>
        /// Indica si el botón de lanzar se mantiene presionado.
        /// </summary>
        public bool IsThrowPressed { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Evento reactivo al completar la acción de interacción.
        /// </summary>
        public event Action OnInteractPerformed;

        /// <summary>
        /// Evento reactivo al completar la acción de lanzamiento.
        /// </summary>
        public event Action OnThrowPerformed;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa los mapas de entrada de gameplay y registra los callbacks.
        /// </summary>
        public InputReader()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Gameplay.SetCallbacks(this);
            _inputActions.Gameplay.Enable();
        }
        #endregion

        #region Interface Implementation (IGameplayActions)
        public void OnMovement(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsRunPressed = true;
            }
            else if (context.canceled)
            {
                IsRunPressed = false;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsInteractPressed = true;
                InteractBuffer.Trigger();
            }
            else if (context.canceled)
            {
                IsInteractPressed = false;
            }

            if (context.performed)
            {
                OnInteractPerformed?.Invoke();
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsThrowPressed = true;
                ThrowBuffer.Trigger();
            }
            else if (context.canceled)
            {
                IsThrowPressed = false;
            }

            if (context.performed)
            {
                OnThrowPerformed?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsCrouchPressed = true;
            }
            else if (context.canceled)
            {
                IsCrouchPressed = false;
            }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Libera los recursos y desactiva las acciones para evitar fugas de memoria.
        /// </summary>
        public void Dispose()
        {
            _inputActions.Gameplay.Disable();
            _inputActions.Gameplay.RemoveCallbacks(this);
            _inputActions.Dispose();
        }
        #endregion
    }
}
