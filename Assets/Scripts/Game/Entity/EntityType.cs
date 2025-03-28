#nullable enable

using game.math;
using game.world;

namespace game.entity
{
	public sealed class EntityType<T> where T : Entity
	{
		public static EntityType<PlayerEntity> Player;


		private IEntityFactory<T> _factory;
		
		public T Spawn(World world, TilePos tilePos, SpawnReason reason)
		{
			//TODO: Implement

			return null;
		}
		
		public T? Create(World world, TilePos tilePos, SpawnReason reason)
		{
			return null;
		}
	}
}