namespace MFA.Character
{
    #region Usings
    //---- Unity --------------
    using UnityEngine;
    //---- MFA ----------------
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    #endregion

    public class PlayerRunState : IState
    {
        #region References
        readonly PlayerContext ctx;
        #endregion

        #region Construct
        public PlayerRunState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region IState Implementation
        public void Enter()
        {
            ctx.Animator.SetBool(AnimationHashes.IsRuning, true);
        }

        public void Tick()
        {
            //---- Idle --------------------------------------------------------------
            if (ctx.Input.MoveDirection.sqrMagnitude <= 0.01f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
                return;
            }

            //---- Move --------------------------------------------------------------
            if (!ctx.Input.IsRunPressed)
            {
                ctx.FSM.ChangeState(ctx.MoveState);
            }

            //---- Interact ----------------------------------------------------------
            if (ctx.Input.IsInteractPressed && ctx.Interaction.CurrentTarget != null)
            {
                ctx.FSM.ChangeState(ctx.InteractState);
                return;
            }
        }

        public void FixedTick()
        {
            Vector2 moveInput = ctx.Input.MoveDirection;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

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
        #endregion
    }
}