namespace MFA.Character
{
    using MFA.Core.FSM;
    using MFA.Core.Interaction;
    using MFA.Core.Physics;
    using MFA.Inputs;
    using UnityEngine;

    public class PlayerInstaller
    {
        public PlayerContext Install
        (
            Rigidbody rb,
            Transform cameraTransform,
            Animator animator,
            CharacterSO settings
        )
        {
            StateMachine fsm = new StateMachine();
            InputReader inputReader = new InputReader();

            PhysicsSystem physics = new PhysicsSystem(settings.PhysicsSettings, rb, cameraTransform);

            InteractionSystem interaction = new InteractionSystem(rb.transform, settings.InteractionSettings);

            PlayerContext ctx = new PlayerContext 
            ( 
                rb,
                cameraTransform,
                animator,
                settings,
                fsm,
                inputReader,
                physics,
                interaction
            );

            ctx.IdleState = new PlayerIdleState(ctx);
            ctx.MoveState = new PlayerMoveState(ctx);
            ctx.RunState = new PlayerRunState(ctx);
            ctx.InteractState = new PlayerInteractState(ctx);

            fsm.ChangeState(ctx.IdleState);

            return ctx;
        }
    }
}
