namespace MFA.Core.Physics
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "SO_Nombre_PhysicsSettings", menuName = "Game/Core/Physics Settings")]
    public class PhysicsSO : ScriptableObject
    {
        [field: Header("Movement")]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 6f;
        [field: SerializeField] public float Acceleration { get; private set; } = 20f;

        [field: Header("Run Movement")]
        [field: SerializeField] public float MaxRunSpeed { get; private set; } = 10f;
        [field: SerializeField] public float RunAcceleration { get; private set; } = 25f;

        [field: Header("Deceleration & Rotation")]
        [field: SerializeField] public float Deceleration { get; private set; } = 25f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 15f;
    }
}