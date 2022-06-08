
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace BoidSim {
	[BurstCompile]
	[UpdateAfter(typeof(BoidClearSystem))]
	internal class BoidInitializerSystem : ComponentSystem {
		protected override void OnUpdate() {
			Entities.ForEach((Entity entity, ref InitializeBoidCommand initializeBoid) => {
				PostUpdateCommands.DestroyEntity(entity);

				var boidSettings = BoidSettings.Active;

				var m_halfWorldSize = boidSettings.HalfWorldSize;

				var halfSpawnRange = m_halfWorldSize * .5f;

				int teamsCount = boidSettings.Teams.Length;

				int boidCount = boidSettings.BoidCount;

				for (uint i = 0; i < boidCount; ++i) {
					var position = new float3(
						URandom.Range(-halfSpawnRange.x, halfSpawnRange.x),
						URandom.Range(-halfSpawnRange.y, halfSpawnRange.y),
						URandom.Range(-halfSpawnRange.z, halfSpawnRange.z)); 

					var velocity = RandomUnitFloat3() * boidSettings.InitialVelocity;

					var team = URandom.Range(0, teamsCount);

					SpawnEntity(position, velocity, team, boidSettings.Teams[team]);
				}
			});
		}

		private void SpawnEntity(float3 position, float3 velocity, int teamIndex, BoidTeam team) {
			var newBoid = EntityManager.CreateEntity();
			EntityManager.AddComponentData(newBoid, new BoidComponentData() { Position = position, Velocity = velocity, Team = teamIndex });
			EntityManager.AddBuffer<SameTeamAvoidanceDynamicBuffer>(newBoid);
			EntityManager.AddBuffer<AllAvoidanceDynamicBuffer>(newBoid);
			EntityManager.AddComponentData(newBoid, new TeamPropertiesComponentData() { Acceleration = team.Acceleration, Drag = team.Drag });

			EntityManager.AddComponent<Translation>(newBoid);
			EntityManager.AddComponent<Rotation>(newBoid);
			EntityManager.AddComponentData(newBoid, new Scale() { Value = team.RenderMesh.Scale });

			RenderMeshUtility.AddComponents(newBoid, EntityManager, team.RenderMesh.RenderMeshDescription);
		}

		private static float3 RandomUnitFloat3() {
			float a = URandom.Range(0, 2f * Mathf.PI);
			float z = URandom.Range(-1, 1);
			float h = math.sqrt(1f - z * z);
			return new float3(h * math.cos(a), h * math.sin(a), z);
		}
	}
}