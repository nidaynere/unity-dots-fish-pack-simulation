
using Unity.Entities;
using Unity.Mathematics;

namespace BoidSim {
	public struct BoidComponentData : IComponentData {
		public float3 Position;
		public float3 Velocity;
		public int Team;
	}
}