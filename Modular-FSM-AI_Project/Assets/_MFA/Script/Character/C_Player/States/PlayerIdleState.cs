using MFA.Core.FSM;

namespace MFA.Character
{
    using UnityEngine;
    public class PlayerIdleState : IState
    {
        readonly PlayerContext ctx;

        public PlayerIdleState(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public void Enter(){}

        public void Tick()
        {
            //---- Move --------------------------------------
            if (ctx.Input.MoveDirection.sqrMagnitude > 0.01f)
            {
                ctx.FSM.ChangeState(ctx.MoveState);
            }
        }

        public void FixedTick()
        {
            // Aplicamos desaceleración para detener el Rigidbody suavemente
            ctx.Physics.HandleMovement(Vector3.zero, Time.fixedDeltaTime);
        }

        public void Exit(){}
    }
}
