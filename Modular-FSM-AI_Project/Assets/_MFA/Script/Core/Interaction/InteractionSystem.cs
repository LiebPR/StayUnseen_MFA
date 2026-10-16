namespace MFA.Core.Interaction
{
    #region Usings
    using UnityEngine;
    #endregion

    public class InteractionSystem
    {
        #region Fields & Properties
        readonly Transform transform;
        readonly InteractionSO config;
        readonly Collider[] hitBuffer;

        public IInteractable CurrentTarget { get; private set; }
        public Transform CurrentTargetTransform { get; private set; }
        #endregion

        public InteractionSystem(Transform transform, InteractionSO config)
        {
            this.transform = transform;
            this.config = config;
            this.hitBuffer = new Collider[config.MaxColliders];
        }

        public void Tick()
        {
            Vector3 center = transform.TransformPoint(config.BoxOffset);

            int count = Physics.OverlapBoxNonAlloc(
                center,
                config.HalfExtents,
                hitBuffer,
                transform.rotation,
                config.InteractableLayer
            );

            IInteractable nearestInteractable = null;
            Transform nearestTransform = null;
            float minDistanceSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (hitBuffer[i] != null && hitBuffer[i].TryGetComponent<IInteractable>(out var interactable))
                {
                    float sqrDist = (hitBuffer[i].transform.position - transform.position).sqrMagnitude;
                    if (sqrDist < minDistanceSqr)
                    {
                        minDistanceSqr = sqrDist;
                        nearestInteractable = interactable;
                        nearestTransform = hitBuffer[i].transform;
                    }
                }
            }

            if (nearestInteractable != CurrentTarget)
            {
                CurrentTarget?.OnFocusLost();
                CurrentTarget = nearestInteractable;
                CurrentTargetTransform = nearestTransform;
                CurrentTarget?.OnFocusGained();
            }
        }

        public void InteractPressed() => CurrentTarget?.OnInteractPressed();
        public void InteractReleased() => CurrentTarget?.OnInteractReleased();

#if UNITY_EDITOR
        public static void DrawGizmos(Transform transform, InteractionSO config)
        {
            if (config == null || transform == null) return;

            Gizmos.color = Color.cyan;
            Matrix4x4 previousMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(config.BoxOffset, config.BoxSize);

            Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
            Gizmos.DrawCube(config.BoxOffset, config.BoxSize);

            Gizmos.matrix = previousMatrix;
        }
#endif
    }
}