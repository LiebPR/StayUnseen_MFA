namespace MFA.Character
{
    using MFA.Core.Animations;
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
            Vector2 moveInput = ctx.Input.MoveDirection;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            ctx.Physics.HandleMovement(moveDirection, Time.fixedDeltaTime);
            ctx.Physics.HandleRotation(moveDirection, Time.fixedDeltaTime);

            float currentSpeed = ctx.Physics.CurrentVelocity.magnitude;

            // Mandamos la velocidad para el blend tree o la transición de walk
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, currentSpeed);

            // Congelamos el flag de correr a false de momento para que se quede en Walking
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }

        public void Exit()
        {
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, 0f);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }
    }
}
