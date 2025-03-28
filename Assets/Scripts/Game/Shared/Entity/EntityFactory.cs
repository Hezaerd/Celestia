using celestia.math;
using celestia.game.shared;

namespace celestia.game.shared
{
	public interface IEntityFactory<out T> where T : Entity
	{
		T Create(World world, TilePos tilePos, SpawnReason reason);
	}
}