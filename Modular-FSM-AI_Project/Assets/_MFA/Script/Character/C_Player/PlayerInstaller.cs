namespace MFA.Character
{
    using MFA.Core.FSM;
    using MFA.Core.Physics;
    using MFA.Inputs;
    using UnityEngine;

    public class PlayerInstaller
    {
        public PlayerContext Install
        (
            Rigidbody rb,
            Transform cameraTransform,
            CharacterSO settings
        )
        {
            StateMachine fsm = new StateMachine();
            InputReader inputReader = new InputReader();

            PhysicsSystem physics = new PhysicsSystem(settings.PhysicsSettings, rb, cameraTransform);

            PlayerContext ctx = new PlayerContext 
            ( 
                rb,
                cameraTransform,
                settings,
                fsm,
                inputReader,
                physics
            );

            ctx.IdleState = new PlayerIdleState(ctx);
            ctx.MoveState = new PlayerMoveState(ctx);

            fsm.ChangeState(ctx.IdleState);

            return ctx;
        }
    }
}
