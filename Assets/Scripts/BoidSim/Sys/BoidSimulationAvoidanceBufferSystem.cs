
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [UpdateAfter (typeof (BoidInitializerSystem))]
    [BurstCompile]
    internal class BoidSimulationAvoidanceBufferSystem : ComponentSystem {

        private static bool isAllocated;
        private NativeArray<Entity> entityArray;
        private NativeArray<BoidComponentData> allAvoidanceResults;
        private NativeArray<BoidComponentData> sameTeamAvoidanceResults;
        private NativeArray<int> allAvoidanceResultCount;
        private NativeArray<int> sameTeamAvoidanceResultCount;

        private bool hasActiveJob;
        protected override void OnUpdate () {
            if (!BoidSettings.Active.AvoidanceBufferSystem) {
                return;
            }

            Entities.ForEach ( (Entity entity, ref DisposeBufferCommand disposer) => {
                PostUpdateCommands.DestroyEntity (entity);

                if (isAllocated) {
                    entityArray.Dispose ();
                    allAvoidanceResults.Dispose ();
                    sameTeamAvoidanceResults.Dispose();
                    sameTeamAvoidanceResultCount.Dispose ();
                    allAvoidanceResultCount.Dispose ();

                    isAllocated = false;
                }
            });

            Entities.ForEach ( (Entity entity, ref AllocateBufferCommand allocator) => {
                PostUpdateCommands.DestroyEntity (entity);

                if (!isAllocated) {
                    entityArray =
                        GetEntityQuery(typeof(BoidComponentData)).
                        ToEntityArray (Allocator.Persistent);

                    int length = entityArray.Length;

                    allAvoidanceResultCount = new NativeArray<int> (length, Allocator.Persistent);
                    sameTeamAvoidanceResultCount = new NativeArray<int> (length, Allocator.Persistent);
                    allAvoidanceResults = new NativeArray<BoidComponentData> (length * AvoidanceParallelJob.MAX_RESULT_CAPACITY, Allocator.Persistent);
                    sameTeamAvoidanceResults = new NativeArray<BoidComponentData> (length * AvoidanceParallelJob.MAX_RESULT_CAPACITY, Allocator.Persistent);

                    isAllocated = true;
                }
            });

            if (!isAllocated || hasActiveJob) {
                return;
            }

            hasActiveJob = true;
            
            var viewRange = BoidSettings.Active.ViewRange;

            var boidArray = GetComponentDataFromEntity<BoidComponentData>(true);
            var allBuffer = GetBufferFromEntity<AllAvoidanceDynamicBuffer>();
            var sameTeamBuffer = GetBufferFromEntity<SameTeamAvoidanceDynamicBuffer>();

            int entitiesLength = entityArray.Length;

            var activeJob = new AvoidanceParallelJob(
            ref entityArray, 
            ref boidArray, 
            ref allAvoidanceResults, 
            ref  sameTeamAvoidanceResults, 
            ref allAvoidanceResultCount, 
            ref sameTeamAvoidanceResultCount, 
            viewRange);

            var jobHandle = activeJob.Schedule(entitiesLength, 1);
            jobHandle.Complete();

            for (int i = 0; i < entitiesLength; ++i) {
                var allAvoidance = allBuffer[entityArray[i]];
                var sameTeamAvoidance = sameTeamBuffer[entityArray[i]];

                // clear buffer.
                allAvoidance.Clear();
                sameTeamAvoidance.Clear();
                //

                // find my result in job.
                var allCount = activeJob.AllAvoidanceResultCount[i];
                var sameTeamCount = activeJob.SameTeamAvoidanceResultCount[i];

                int myStart = AvoidanceParallelJob.MAX_RESULT_CAPACITY * i;

                for (int j = 0; j<allCount; ++j) {
                    allAvoidance.Add(activeJob.AllAvoidanceResults[myStart + j]);
                }

                for (int j = 0; j < sameTeamCount; ++j) {
                    sameTeamAvoidance.Add(activeJob.SameTeamAvoidanceResults[myStart + j]);
                }
            }
            
            hasActiveJob = false;
        }
    }
}