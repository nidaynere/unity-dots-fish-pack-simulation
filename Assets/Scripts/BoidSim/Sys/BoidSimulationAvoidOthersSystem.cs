
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    
    [UpdateAfter (typeof (BoidSimulationUpdateCoherenceSystem))]
    internal class BoidSimulationAvoidOthersSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            if (!BoidSettings.Active.AvoidOthersSystem) {
                return inputDeps;
            }

            float avoidanceRate = BoidSettings.Active.AvoidanceRate;
            float avoidanceRange = BoidSettings.Active.AvoidanceRange;
            float dt = Time.DeltaTime;

            var minDistSqr = math.pow(avoidanceRange, 2);

            inputDeps = Entities.ForEach(
                (
                ref Entity entity,
                ref BoidComponentData source,
                ref DynamicBuffer<AllAvoidanceDynamicBuffer> allAvoidance
                ) => {
                    var m_source = source;

                    int bufferLength = allAvoidance.Length;

                    if (bufferLength == 0) {
                        return;
                    }

                    var myPosition = source.Position;
                    float3 step = float3.zero;

                    for (int i = 0; i < bufferLength; ++i) {
                        var delta = myPosition - allAvoidance[i].Boid.Position;
                        var deltaSqr = math.lengthsq(delta);
                        if (deltaSqr > 0 && deltaSqr < minDistSqr) {
                            step += delta / math.sqrt(deltaSqr);
                        }
                    }

                    source.Velocity += step * avoidanceRate * dt;
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
