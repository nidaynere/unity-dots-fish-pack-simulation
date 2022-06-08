
using Unity.Entities;
using Unity.Mathematics;

namespace BoidSim {
    [InternalBufferCapacity(AvoidanceParallelJob.MAX_RESULT_CAPACITY)]
    internal struct AllAvoidanceDynamicBuffer : IBufferElementData {
        public static implicit operator BoidComponentData(AllAvoidanceDynamicBuffer e) { return e; }
        public static implicit operator AllAvoidanceDynamicBuffer(BoidComponentData e) { return new AllAvoidanceDynamicBuffer { Boid = e }; }

        public BoidComponentData Boid;
    }
}
