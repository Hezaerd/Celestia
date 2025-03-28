using game.math;

namespace game
{
	public class SpawnPoint
	{
		private readonly TilePos _pos;
		
		public SpawnPoint(TilePos pos)
		{
			_pos = pos;
		}
		
		// Implicit conversion from SpawnPoint to TilePos
		public static implicit operator TilePos(SpawnPoint spawnPoint)
		{
			return spawnPoint._pos;
		}
	}
}