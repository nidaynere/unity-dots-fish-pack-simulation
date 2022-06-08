
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    [UpdateAfter (typeof (BoidSimulationAvoidanceBufferSystem))]
    internal class BoidSimulationAvoidInsideBoundsOfCubeSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.AvoidInsideBoundsOfCubeSystem) {
                return inputDeps;
            }

            float dt = Time.DeltaTime;
            float avoidRange = BoidSettings.Active.ViewRange;
            float3 halfCubeSize = BoidSettings.Active.HalfWorldSize;

            return Entities.ForEach((ref BoidComponentData boid) => {
                boid.Velocity -= new float3(
                    math.max(math.abs(boid.Position.x) - halfCubeSize.x + avoidRange, 0) * math.sign(boid.Position.x) * 5 * dt,
                    math.max(math.abs(boid.Position.y) - halfCubeSize.y + avoidRange, 0) * math.sign(boid.Position.y) * 5 * dt,
                    math.max(math.abs(boid.Position.z) - halfCubeSize.z + avoidRange, 0) * math.sign(boid.Position.z) * 5 * dt);
            }).Schedule(inputDeps);
        }
    }
}