
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace BoidSim {
    [BurstCompile]
    internal class BoidTransformSystem : JobComponentSystem {
        private readonly static float3 UP = new float3(0, 1, 0);

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.TransformSystem) {
                return inputDeps;
            }

            float deltaTime = Time.DeltaTime * 10;

            inputDeps = Entities.ForEach((
                ref Translation translation, 
                ref Rotation rotation,
                ref BoidComponentData boidComponent) => {

                    translation.Value = boidComponent.Position;

                    var targetRot = quaternion.LookRotation(boidComponent.Velocity, UP);

                    rotation.Value = math.slerp(rotation.Value, targetRot, deltaTime);

                }).Schedule (inputDeps);

            return inputDeps;
        }
    }
}
