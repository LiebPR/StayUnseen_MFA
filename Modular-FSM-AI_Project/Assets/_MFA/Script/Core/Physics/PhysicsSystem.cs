namespace MFA.Core.Physics
{
    using UnityEngine;

    public class PhysicsSystem
    {
        #region Fields
        readonly PhysicsSO settings;
        readonly Rigidbody rigidbody;
        readonly Transform cameraTransform;

        Vector3 currentVelocity;
        float currentSpeed;

        public Vector3 CurrentVelocity => currentVelocity;
        #endregion

        #region Construct
        public PhysicsSystem(PhysicsSO settings, Rigidbody rigidbody, Transform cameraTransform)
        {
            this.settings = settings;
            this.rigidbody = rigidbody;
            this.cameraTransform = cameraTransform;
        }
        #endregion

        #region API
        public void HandleMovement(Vector3 inputDirection, float fixedDeltaTime)
        {
            // Adaptar el input a la perspectiva de la cámara del Dolly Track
            Vector3 moveDirection = GetCameraRelativeDirection(inputDirection);

            // Cálculo de velocidad con inercia y control de inversión de giro
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                float dotProduct = Vector3.Dot(currentVelocity.normalized, moveDirection);
                float currentAccel = settings.Acceleration;

                // Si el jugador frena en seco o gira 180 grados, aumentamos temporalmente la respuesta
                if (dotProduct < -0.2f && currentSpeed > 0.5f)
                {
                    currentAccel *= 1.8f;
                }

                currentSpeed = Mathf.MoveTowards(currentSpeed, settings.MaxSpeed, currentAccel * fixedDeltaTime);
                currentVelocity = moveDirection.normalized * currentSpeed;
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, settings.Deceleration * fixedDeltaTime);
                currentVelocity = currentVelocity.normalized * currentSpeed;
            }

            // Aplicar velocidad horizontal manteniendo la vertical del Rigidbody (gravedad)
            Vector3 targetVelocity = new Vector3(currentVelocity.x, rigidbody.linearVelocity.y, currentVelocity.z);
            rigidbody.linearVelocity = targetVelocity;
        }

        public void HandleRotation(Vector3 inputDirection, float fixedDeltaTime)
        {
            Vector3 moveDirection = GetCameraRelativeDirection(inputDirection);

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                Quaternion newRotation = Quaternion.Slerp(
                    rigidbody.rotation,
                    targetRotation,
                    settings.RotationSpeed * fixedDeltaTime
                );

                rigidbody.MoveRotation(newRotation);
            }
        }

        private Vector3 GetCameraRelativeDirection(Vector3 input)
        {
            if (cameraTransform == null) return input.normalized;

            // Proyectar los ejes de la cámara sobre el plano horizontal (XZ)
            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            return (camForward * input.z + camRight * input.x).normalized;
        }
        #endregion
    }
}