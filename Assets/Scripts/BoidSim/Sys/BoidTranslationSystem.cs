
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace BoidSim {
    [BurstCompile]
    internal class BoidTranslationSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            float dt = Time.DeltaTime;

            inputDeps = Entities.ForEach((
                ref BoidComponentData boidComponent) => {
                    boidComponent.Position += boidComponent.Velocity * dt;
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
