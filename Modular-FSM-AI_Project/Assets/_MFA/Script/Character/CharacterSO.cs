namespace MFA.Character
{
    using MFA.Core.Physics;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SO_Nombre_CharacterSettings", menuName = "Game/Character/Settings Container")]
    public class CharacterSO : ScriptableObject
    {
        public PhysicsSO PhysicsSettings;
    }
}
