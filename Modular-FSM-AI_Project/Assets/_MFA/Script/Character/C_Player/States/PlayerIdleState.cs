namespace MFA.Character
{
    using MFA.Core.Animations;
    using MFA.Core.FSM;
    using UnityEngine;

    public class PlayerIdleState : IState
    {
        readonly PlayerContext ctx;

        public PlayerIdleState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public void Enter()
        {
            // Aseguramos que al entrar en Idle se limpien los flags del animator
            ctx.Animator.SetFloat(AnimationHashes.MoveSpeed, 0f);
            ctx.Animator.SetBool(AnimationHashes.IsRuning, false);
        }

        public void Tick()
        {
            if (ctx.Input.MoveDirection.sqrMagnitude > 0.01f)
            {
                ctx.FSM.ChangeState(ctx.MoveState);
            }
        }

        public void FixedTick()
        {
            ctx.Physics.HandleMovement(Vector3.zero, Time.fixedDeltaTime);
        }

        public void Exit() { }
    }
}