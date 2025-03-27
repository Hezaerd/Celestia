using Game.World;
using UnityEngine;

namespace Game.Maths
{
	public static class Isometric
	{
		#region Constants
		
		public const float TileWidth = 1.0f;
		public const float TileHeight = 0.5f;
		
		#endregion
		
		#region Cartesian
		
		/// <summary>
		/// Converts a cartesian position to an isometric position.
		/// </summary>
		/// <param name="cartesianPos">The cartesian position to convert.</param>
		/// <returns>The Isometric coordinate.</returns>
		public static Vector2 CartesianToIso(Vector2 cartesianPos)
		{
			var x = cartesianPos.x - cartesianPos.y;
			var y = (cartesianPos.x + cartesianPos.y) * 0.5f;
			
			return new Vector2(x, y);
		}
		
		/// <summary>
		/// Converts a cartesian position to an isometric position.
		/// </summary>
		/// <param name="cartesianPos">The cartesian position to convert.</param>
		/// <returns>The Isometric coordinate.</returns>
		public static Vector3 CartesianToIso(Vector3 cartesianPos)
		{
			var x = cartesianPos.x - cartesianPos.y;
			var y = (cartesianPos.x + cartesianPos.y) * 0.5f;
			
			return new Vector3(x, y, 0);
		}
		
		/// <summary>
		/// Converts an isometric position to a cartesian position.
		/// </summary>
		/// <param name="isoPos">The isometric position to convert.</param>
		/// <returns>The cartesian coordinate.</returns>
		public static Vector2 IsoToCartesian(Vector2 isoPos)
		{
			var x = (2 * isoPos.y + isoPos.x) * 0.5f;
			var y = (2 * isoPos.y - isoPos.x) * 0.5f;
			
			return new Vector2(x, y);
		}
		
		/// <summary>
		/// Converts an isometric position to a cartesian position.
		/// </summary>
		/// <param name="isoPos">The isometric position to convert.</param>
		/// <returns>The cartesian coordinate.</returns>
		public static Vector3 IsoToCartesian(Vector3 isoPos)
		{
			var x = (2 * isoPos.y + isoPos.x) * 0.5f;
			var y = (2 * isoPos.y - isoPos.x) * 0.5f;
			
			return new Vector3(x, y, 0);
		}
		
		#endregion
		
		#region TilePos
		
		public static TilePos WorldToTile(Vector3 worldPos)
		{
			var x = Mathf.FloorToInt(worldPos.x / TileWidth);
			var y = Mathf.FloorToInt(worldPos.y / TileHeight);
			
			return new TilePos(x, y);
		}
		
		public static Vector3 TileToWorld(TilePos tilePos)
		{
			var worldX = (tilePos.x - tilePos.y) * TileWidth * 0.5f;
			var worldY = (tilePos.x + tilePos.y) * TileHeight * 0.5f;
			
			return new Vector3(worldX, worldY, 0);
		}
		
		#endregion
	}
}