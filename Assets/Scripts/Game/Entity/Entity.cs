using game.math;
using game.world;
using UnityEngine;

namespace game.entity
{
	public abstract class Entity : MonoBehaviour
	{
		protected World World;
		protected TilePos Pos;
		
		public World GetWorld()
		{
			return World;
		}

	}
}