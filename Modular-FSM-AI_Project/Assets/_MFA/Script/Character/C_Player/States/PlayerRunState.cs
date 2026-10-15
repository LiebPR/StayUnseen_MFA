namespace MFA.Character
{
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    using UnityEngine;

    public class PlayerRunState : IState
    {
        readonly PlayerContext ctx;

        public PlayerRunState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public void Enter()
        {
            ctx.Animator.SetBool(AnimationHashes.IsRuning, true);
        }

        public void Tick()
        {
            if (ctx.Input.MoveDirection.sqrMagnitude <= 0.01f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
                return;
            }

            // Si suelta el botón de correr pero sigue moviéndose, regresa a caminar
            if (!ctx.Input.IsRunPressed)
            {
                ctx.FSM.ChangeState(ctx.MoveState);
            }
        }

        public void FixedTick()
        {
            Vector2 moveInput = ctx.Input.MoveDirection;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            // Inyectamos las variables de velocidad de carrera del ScriptableObject
            ctx.Physics.HandleMovement(
                moveDirection,
                ctx.Settings.PhysicsSettings.MaxRunSpeed,
                ctx.Settings.PhysicsSettings.RunAcceleration,
                Time.fixedDeltaTime
            );

            ctx.Physics.HandleRotation(moveDirection, Time.fixedDeltaTime);

            float currentSpeed = ctx.Physics.CurrentVelocity.magnitude;
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, currentSpeed);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, true);
        }

        public void Exit()
        {
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }
    }
}