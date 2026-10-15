namespace MFA.Character
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CharacterSO settings;
        [SerializeField] Transform cameraTransform;

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

            //---- Aseguramos la cámara ---------------------------
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            //---- Instanciamos el Installer y el CTX -------------
            PlayerInstaller installer = new PlayerInstaller();
            ctx = installer.Install(rb, cameraTransform, settings);

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
