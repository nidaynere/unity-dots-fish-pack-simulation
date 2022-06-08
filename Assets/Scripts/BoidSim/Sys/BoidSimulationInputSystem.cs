
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BoidSim {
    [BurstCompile]
    internal class BoidSimulationInputSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            var velPower = 50;

            var input = new float3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0);

            float dt = Time.DeltaTime;

            inputDeps = Entities.ForEach(
                (
                    ref Entity entity,
                    ref BoidComponentData source
                ) => {
                    source.Velocity += input * dt * velPower;
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
