namespace MFA.Character
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        #region References
        PlayerContext ctx;
        #endregion

        private void Awake()
        {
            //----- Aseguramos el Rigidbody -------------------
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            //---- Instanciamos el Installer y el CTX ----------------------------------
            PlayerInstaller installer = new PlayerInstaller();
            ctx = installer.Install(rb);

        }

        void Update()
        {
            ctx.FSM.Update();        
        }

        void FixedUpdate()
        {
            ctx.FSM.FixedUpdate();
        }

        private void OnDestroy()
        {
            ctx.Dispose();
        }
    }
}
