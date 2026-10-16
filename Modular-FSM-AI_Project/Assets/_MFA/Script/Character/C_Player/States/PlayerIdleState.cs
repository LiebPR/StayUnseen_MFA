namespace MFA.Character
{
    #region Usings
    //---- Unity --------------
    using UnityEngine;
    //---- MFA ----------------
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    #endregion

    public class PlayerIdleState : IState
    {
        #region References
        readonly PlayerContext ctx;
        #endregion

        #region Construct
        public PlayerIdleState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region IState Implementation
        public void Enter()
        {
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, 0f);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }

        public void Tick()
        {
            if (ctx.Input.MoveDirection.sqrMagnitude > 0.01f)
            {
                //---- Move ----------------------------------------------------------
                if (!ctx.Input.IsRunPressed)
                {
                    ctx.FSM.ChangeState(ctx.MoveState);
                }

                //---- Run -----------------------------------------------------------
                if (ctx.Input.IsRunPressed)
                {
                    ctx.FSM.ChangeState(ctx.RunState);
                }
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
            ctx.Physics.HandleMovement(Vector3.zero, Time.fixedDeltaTime);
        }

        public void Exit() { }
        #endregion
    }
}