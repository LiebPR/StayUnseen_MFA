namespace MFA.Character
{
    using MFA.Core.FSM;
    using MFA.Inputs;
    using UnityEngine;

    public class PlayerInstaller
    {
        public PlayerContext Install
        (
            Rigidbody rb
        )
        {
            StateMachine fsm = new StateMachine ();
            InputReader inputReader = new InputReader ();

            PlayerContext ctx = new PlayerContext 
            ( 
                rb,
                fsm,
                inputReader
            );
            return ctx;
        }
    }
}
