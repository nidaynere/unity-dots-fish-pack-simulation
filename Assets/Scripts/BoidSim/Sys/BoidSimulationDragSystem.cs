
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    [UpdateAfter (typeof (BoidSimulationAvoidOthersSystem))]
    internal class BoidSimulationAccelerationAndDragSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.DragSystem) {
                return inputDeps;
            }

            float dt = Time.DeltaTime;

            inputDeps = Entities.ForEach(
                (
                    ref Entity entity,
                    ref BoidComponentData source,
                    ref TeamPropertiesComponentData teamProperties
                ) => {
                    // Acceleration and drag
                    float3 velocity = source.Velocity;
                    velocity += math.normalize(velocity) * teamProperties.Acceleration * dt;
                    velocity *= 1.0f - 30.0f * teamProperties.Drag * dt;

                    // Update the velocity.
                    source.Velocity = velocity;
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
