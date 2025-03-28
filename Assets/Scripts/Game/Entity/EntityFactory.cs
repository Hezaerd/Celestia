using game.entity;
using game.math;
using game.world;

namespace game.entity
{
	public interface IEntityFactory<out T> where T : Entity
	{
		T Create(World world, TilePos tilePos, SpawnReason reason);
	}
}