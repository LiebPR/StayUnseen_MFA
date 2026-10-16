namespace MFA.Character
{
    using MFA.Core.Interaction;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CharacterSO settings;
        [SerializeField] Transform cameraTransform;
        [SerializeField] Animator animator;

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
            ctx = installer.Install(rb, cameraTransform, animator, settings);

        }

        void Update()
        {
            ctx.FSM.Update();
            ctx.Interaction.Tick();
        }

        void FixedUpdate()
        {
            ctx.FSM.FixedUpdate();
        }

        private void OnDestroy()
        {
            ctx.Dispose();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            InteractionSystem.DrawGizmos(transform, settings.InteractionSettings);
        }
#endif
    }
}
