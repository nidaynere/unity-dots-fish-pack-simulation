
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    
    [UpdateAfter (typeof (BoidSimulationMatchVelocitySystem))]
    internal class BoidSimulationUpdateCoherenceSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.UpdateCoherenceSystem) {
                return inputDeps;
            }

            float coherenceRate = BoidSettings.Active.CoherenceRate;
            float dt = Time.DeltaTime;

            inputDeps = Entities.ForEach((
                ref Entity entity,
                ref BoidComponentData source,
                ref DynamicBuffer<SameTeamAvoidanceDynamicBuffer> sameTeamAvoidance) => {

                    var m_source = source;

                    int bufferLength = sameTeamAvoidance.Length;

                    if (bufferLength == 0) {
                        return;
                    }

                    float3 center = sameTeamAvoidance[0].Boid.Position;
                    for (int i = 1; i < bufferLength; ++i) {
                        center += sameTeamAvoidance[i].Boid.Position;
                    }

                    center *= 1.0F / bufferLength;

                    var velocityAdd = (center - source.Position) * coherenceRate * dt;

                    source.Velocity += velocityAdd;
                }).
                Schedule(inputDeps);

            return inputDeps;
        }
    }
}