
using Unity.Entities;

namespace BoidSim {
    internal struct TeamPropertiesComponentData : IComponentData {
        public float Acceleration;
        public float Drag;
    }
}
