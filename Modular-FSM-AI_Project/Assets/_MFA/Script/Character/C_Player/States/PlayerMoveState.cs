namespace MFA.Character
{
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    using UnityEngine;

    public class PlayerMoveState : IState
    {
        readonly PlayerContext ctx;

        public PlayerMoveState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public void Enter() { }

        public void Tick()
        {
            // Si no hay input, volvemos a Idle
            if (ctx.Input.MoveDirection.sqrMagnitude <= 0.01f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
            }
        }

        public void FixedTick()
        {
            Vector2 moveInput = ctx.Input.MoveDirection;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            // Determinamos si corre o camina en base al input actual (Sin cambios de estado en FSM innecesarios)
            bool isRunning = ctx.Input.IsRunPressed;

            float targetMaxSpeed = isRunning ? ctx.Settings.PhysicsSettings.MaxRunSpeed : ctx.Settings.PhysicsSettings.MaxSpeed;
            float targetAccel = isRunning ? ctx.Settings.PhysicsSettings.RunAcceleration : ctx.Settings.PhysicsSettings.Acceleration;

            // Aplicamos físicas usando la sobrecarga limpia
            ctx.Physics.HandleMovement(moveDirection, targetMaxSpeed, targetAccel, Time.fixedDeltaTime);
            ctx.Physics.HandleRotation(moveDirection, Time.fixedDeltaTime);

            float currentSpeed = ctx.Physics.CurrentVelocity.magnitude;

            // Actualizamos parámetros del Animator de una sola pasada
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, currentSpeed);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, isRunning);
        }

        public void Exit()
        {
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, 0f);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }
    }
}