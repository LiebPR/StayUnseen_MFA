namespace MFA.Character
{
    #region Usings
    //---- Unity --------------
    using UnityEngine;
    //---- MFA ----------------
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    #endregion

    public class PlayerMoveState : IState
    {
        #region References
        readonly PlayerContext ctx;
        #endregion

        #region Construct
        public PlayerMoveState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region IState Implementation
        public void Enter() { }

        public void Tick()
        {
            //---- Idle --------------------------------------------------------------
            if (ctx.Input.MoveDirection.sqrMagnitude <= 0.01f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
            }

            //---- Run ---------------------------------------------------------------
            if (ctx.Input.IsRunPressed)
            {
                ctx.FSM.ChangeState(ctx.RunState);
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

            bool isRunning = ctx.Input.IsRunPressed;

            float targetMaxSpeed = isRunning ? ctx.Settings.PhysicsSettings.MaxRunSpeed : ctx.Settings.PhysicsSettings.MaxSpeed;
            float targetAccel = isRunning ? ctx.Settings.PhysicsSettings.RunAcceleration : ctx.Settings.PhysicsSettings.Acceleration;

            ctx.Physics.HandleMovement(moveDirection, targetMaxSpeed, targetAccel, Time.fixedDeltaTime);
            ctx.Physics.HandleRotation(moveDirection, Time.fixedDeltaTime);

            float currentSpeed = ctx.Physics.CurrentVelocity.magnitude;

            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, currentSpeed);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, isRunning);
        }

        public void Exit()
        {
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, 0f);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }
        #endregion
    }
}