namespace MFA.Character
{
    using MFA.Core.FSM;
    using UnityEngine;

    /// <summary>
    /// Representa el estado de movimiento (Walk/Run) del personaje.
    /// </summary>
    public class PlayerMoveState : IState
    {
        readonly PlayerContext ctx;

        public PlayerMoveState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public void Enter()
        {
            // Aquí puedes activar la animación de movimiento
        }

        public void Tick()
        {
            // Si el input de movimiento cesa, regresamos al estado de Idle
            if (ctx.Input.MoveDirection.sqrMagnitude <= 0.01f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
            }
        }

        public void FixedTick()
        {
            // Vector2 a Vector3 (en el plano XZ). Nota: Vector3 es un struct (valor), 
            // por lo que no genera basura en el Heap (Zero GC Allocations).
            Vector2 moveInput = ctx.Input.MoveDirection;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            // Delegamos el movimiento y la rotación al sistema físico[cite: 5]
            ctx.Physics.HandleMovement(moveDirection, Time.fixedDeltaTime);
            ctx.Physics.HandleRotation(moveDirection, Time.fixedDeltaTime);
        }

        public void Exit()
        {
            // Limpieza al abandonar el estado de movimiento
        }
    }
}
