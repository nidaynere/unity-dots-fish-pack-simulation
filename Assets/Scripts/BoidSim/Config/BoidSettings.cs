using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BoidSim {
    [CreateAssetMenu(fileName = "BoidSettings", menuName = "Create boid settings", order = 1)]
    public class BoidSettings : ScriptableObject {
	    private const float ROUND_WORLD_SIZE_TO_MULTIPLES_OF = 5;

        public static BoidSettings Active;

        public Vector3 WorldSize => new Vector3 (axisLength, 1, axisLength);
        private float axisLength => Mathf.CeilToInt(Mathf.Pow(BoidCount, 1f / 3) * BoidDensity / ROUND_WORLD_SIZE_TO_MULTIPLES_OF) * ROUND_WORLD_SIZE_TO_MULTIPLES_OF;
        
        [Header("Generation Settings. Needs to Refresh to apply changes.")]
        public int BoidCount;
        [SerializeField] private float BoidDensity = 4f;
        public BoidTeam[] Teams;
        public float3 HalfWorldSize => new float3 (WorldSize.x * 0.5f, WorldSizeY, WorldSize.z * 0.5f);

        public float WorldSizeY = 5;

        [Header("Dynamic settings.")]
        public float InitialVelocity = 2.0f;
        public float MatchVelocityRate = 1.0f;
        public float AvoidanceRange = 2.0f;
        public float AvoidanceRate = 5.0f;
        public float CoherenceRate = 2.0f;
        public float ViewRange = 3.0f;

        [Header("Systems")]
        public bool AvoidanceBufferSystem = true;
        public bool AvoidInsideBoundsOfCubeSystem = true;
        public bool AvoidOthersSystem = true;
        public bool DragSystem = true;
        public bool MatchVelocitySystem = true;
        public bool UpdateCoherenceSystem = true;
        public bool TransformSystem = true;

        public void Activate () {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Active = this;

            var clearEntity = entityManager.CreateEntity();
            entityManager.AddComponent<ClearBoidCommand>(clearEntity);

            var initializeEntity = entityManager.CreateEntity();
            entityManager.AddComponent<InitializeBoidCommand>(initializeEntity);

            var disposer = entityManager.CreateEntity ();
            entityManager.AddComponent<DisposeBufferCommand>(disposer);

            var allocator = entityManager.CreateEntity ();
            entityManager.AddComponent<AllocateBufferCommand>(allocator);
        }
    }
}
