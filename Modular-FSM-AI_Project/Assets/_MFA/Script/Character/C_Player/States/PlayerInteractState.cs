namespace MFA.Character
{
    #region Usings
    //---- Unity --------------
    using UnityEngine;
    //---- MFA ----------------
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    #endregion

    public class PlayerInteractState : IState
    {
        #region Fields
        readonly PlayerContext ctx;

        private const float ChestHeightThreshold = 0.8f;
        #endregion

        #region Construct
        public PlayerInteractState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region IState Implementation
        public void Enter()
        {
            ctx.Physics.HandleMovement(Vector3.zero, Time.fixedDeltaTime);

            float interactTypeVal = 0f;

            if (ctx.Interaction.CurrentTargetTransform != null)
            {
                float relativeY = ctx.Interaction.CurrentTargetTransform.position.y - ctx.Rigidbody.position.y;

                bool isChestHeight = relativeY >= ChestHeightThreshold;
                interactTypeVal = isChestHeight ? 0f : 1f;
            }

            ctx.Animator.SetFloat(AnimationHashes.InteractType, interactTypeVal);
            ctx.Animator.SetTrigger(AnimationHashes.OnInteract);

            ctx.Interaction.InteractPressed();
        }

        public void Tick()
        {
            AnimatorStateInfo stateInfo = ctx.Animator.GetCurrentAnimatorStateInfo(0);

            //---- Idle --------------------------------------------------------------
            if (stateInfo.IsName("Interaction") && stateInfo.normalizedTime >= 0.99f)
            {
                ctx.FSM.ChangeState(ctx.IdleState);
            }
        }

        public void FixedTick()
        {
            ctx.Physics.HandleMovement(Vector3.zero, Time.fixedDeltaTime);
        }

        public void Exit()
        {
            ctx.Interaction.InteractReleased();
        }
        #endregion
    }
}