using Unity.Entities;
using Unity.Mathematics;

namespace BoidSim {
    [InternalBufferCapacity(AvoidanceParallelJob.MAX_RESULT_CAPACITY)]
    internal struct SameTeamAvoidanceDynamicBuffer : IBufferElementData {
        public static implicit operator BoidComponentData(SameTeamAvoidanceDynamicBuffer e) { return e; }
        public static implicit operator SameTeamAvoidanceDynamicBuffer(BoidComponentData e) { return new SameTeamAvoidanceDynamicBuffer { Boid = e }; }

        public BoidComponentData Boid;
    }
}