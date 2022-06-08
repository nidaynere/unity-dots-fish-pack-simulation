
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoidSim {
    [BurstCompile]
    internal struct AvoidanceParallelJob : IJobParallelFor {
        /// <summary>
        /// Increasing this will affect the performance but will create better collision avoidance.
        /// </summary>
        public const int MAX_RESULT_CAPACITY = 16;

        /// <summary>
        /// All entities that have BoidComponentData
        /// </summary>
        [ReadOnly]
        private NativeArray<Entity> entities;
        [ReadOnly]
        private ComponentDataFromEntity<BoidComponentData> boidData;
        [ReadOnly]
        private int length;
        [ReadOnly]
        private float range;

        #region results
        [NativeDisableParallelForRestriction]
        public NativeArray<BoidComponentData> AllAvoidanceResults;
        [NativeDisableParallelForRestriction]
        public NativeArray<int> AllAvoidanceResultCount;
        [NativeDisableParallelForRestriction]
        public NativeArray<BoidComponentData> SameTeamAvoidanceResults;
        [NativeDisableParallelForRestriction]
        public NativeArray<int> SameTeamAvoidanceResultCount;
        #endregion

        public void Execute (int index) {
            var source = boidData[entities[index]];
            int sourceCapacityStart = MAX_RESULT_CAPACITY * index;

            int sourceAllCount = 0;
            int sourceSameTeamCount = 0;

            for (int i=index+1; i<length; ++i) {
                // capacity check.
                if (sourceAllCount == MAX_RESULT_CAPACITY || sourceSameTeamCount == MAX_RESULT_CAPACITY) {
                    break; // reached avoidance capacity.
                }

                var other = boidData[entities[i]];

                int otherAllCount = AllAvoidanceResultCount[i];
                int otherSameTeamCount = SameTeamAvoidanceResultCount[i];

                var otherCapacityStart = MAX_RESULT_CAPACITY * i;

                float dif = math.lengthsq (other.Position - source.Position);

                if (dif < range) {
                    // in radius.
                    AllAvoidanceResults[sourceCapacityStart + sourceAllCount++] = other;

                    if (otherAllCount < MAX_RESULT_CAPACITY) {
                        AllAvoidanceResults[otherCapacityStart + otherAllCount ++] = source;
                        AllAvoidanceResultCount[i] = otherAllCount;
                    }

                    if (source.Team == other.Team) {
                        // same team.
                        SameTeamAvoidanceResults[sourceCapacityStart + sourceSameTeamCount++] = other;

                        if (otherSameTeamCount < MAX_RESULT_CAPACITY) {
                            SameTeamAvoidanceResults[otherCapacityStart + otherSameTeamCount ++] = source;
                            SameTeamAvoidanceResultCount[i] = otherSameTeamCount;
                        }
                    }
                }
            }

            AllAvoidanceResultCount[index] = sourceAllCount;
            SameTeamAvoidanceResultCount[index] = sourceSameTeamCount;
        }

        public AvoidanceParallelJob (
            [ReadOnly] ref NativeArray<Entity> entities,
            [ReadOnly] ref ComponentDataFromEntity<BoidComponentData> boidData,
            ref NativeArray<BoidComponentData> allAvoidanceResultPositions,
            ref NativeArray<BoidComponentData> sameTeamAvoidanceResultPositions,
            ref NativeArray<int> allAvoidanceResultCount,
            ref NativeArray<int> sameTeamAvoidanceResultCount,
            in float range) {

            this.length = entities.Length;

            this.AllAvoidanceResultCount = allAvoidanceResultCount;
            this.SameTeamAvoidanceResultCount = sameTeamAvoidanceResultCount;
            this.AllAvoidanceResults = allAvoidanceResultPositions;
            this.SameTeamAvoidanceResults = sameTeamAvoidanceResultPositions;

            this.range = range * range; // sqr
            this.entities = entities;
            this.boidData = boidData;
        }
    }
}
