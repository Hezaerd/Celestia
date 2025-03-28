using celestia.math;
using UnityEngine;

namespace celestia.game.shared
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