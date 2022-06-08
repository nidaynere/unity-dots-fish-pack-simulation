
using Unity.Burst;
using Unity.Entities;

namespace BoidSim {
	[BurstCompile]
	internal class BoidClearSystem : ComponentSystem {
		protected override void OnUpdate() {
			var clearCommand = false;

			Entities.ForEach((Entity entity, ref ClearBoidCommand initializeBoid) => {
				clearCommand = true;
				PostUpdateCommands.DestroyEntity(entity);
			});

			if (clearCommand) {
				Entities.ForEach((Entity entity, ref BoidComponentData initializeBoid) => {
					PostUpdateCommands.DestroyEntity(entity);
				});
			}
		}
	}
}