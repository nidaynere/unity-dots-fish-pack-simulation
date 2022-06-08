
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    
    [UpdateAfter (typeof (BoidSimulationAvoidInsideBoundsOfCubeSystem))]
    internal class BoidSimulationMatchVelocitySystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.MatchVelocitySystem) {
                return inputDeps;
            }

            float matchRate = BoidSettings.Active.MatchVelocityRate;
            float dt = Time.DeltaTime;

            inputDeps = Entities.ForEach(
                (
                ref Entity entity,
                ref BoidComponentData source,
                ref DynamicBuffer<SameTeamAvoidanceDynamicBuffer> sameTeamAvoidance
                ) => {
                    int bufferLength = sameTeamAvoidance.Length;

                    if (bufferLength == 0) {
                        return;
                    }

                    float3 velocity = new float3 (0,0,0);
                    for (int i = 0; i < bufferLength; ++i) {
                        velocity += sameTeamAvoidance[i].Boid.Velocity;
                    }

                    velocity /= bufferLength;
                    
                    source.Velocity += (velocity - source.Velocity) * matchRate * dt;
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
