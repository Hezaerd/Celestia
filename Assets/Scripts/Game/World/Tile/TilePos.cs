using System;
using UnityEngine;

namespace Game.World
{
	/// <summary>
	/// Represents a tile coordinate in the game world.
	/// </summary>
	[Serializable]
	public class TilePos : IEquatable<TilePos>
	{
		/// <summary>
		/// X coordinate.
		/// </summary>
		public int x;

		/// <summary>
		/// Y coordinate.
		/// </summary>
		public int y;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public TilePos(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Returns a string representation of the tile coordinate.
		/// </summary>
		/// <returns>String representation of the tile coordinate.</returns>
		public override string ToString()
		{
			return $"{x}, {y}";
		}
		
		#region Equality Operators
		
		public static bool operator==(TilePos a, TilePos b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
				return false;

			return a.x == b.x && a.y == b.y;
		}
		
		public static bool operator!=(TilePos a, TilePos b)
		{
			return !(a == b);
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			TilePos other = (TilePos) obj;
			return x == other.x && y == other.y;
		}
		
		public bool Equals(TilePos other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return x == other.x && y == other.y;
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(x, y);
		}
		
		#endregion
		
		#region Arithmetic Operators
		
		public static TilePos operator+(TilePos a, TilePos b)
		{
			return new TilePos(a.x + b.x, a.y + b.y);
		}
		
		public static TilePos operator-(TilePos a, TilePos b)
		{
			return new TilePos(a.x - b.x, a.y - b.y);
		}
		
		public static TilePos operator*(TilePos a, int scalar)
		{
			return new TilePos(a.x * scalar, a.y * scalar);
		}
		
		public static TilePos operator/(TilePos a, int scalar)
		{
			return new TilePos(a.x / scalar, a.y / scalar);
		}
		
		#endregion
		
		#region Vec2Int Conversion
		
		public static implicit operator TilePos(Vector2Int vec2)
		{
			return new TilePos(vec2.x, vec2.y);
		}
		
		public static implicit operator Vector2Int(TilePos tilePos)
		{
			return new Vector2Int(tilePos.x, tilePos.y);
		}
		
		#endregion
		
		#region Vec2 Conversion
		
		public static implicit operator TilePos(Vector2 vec2)
		{
			return new TilePos((int) vec2.x, (int) vec2.y);
		}
		
		public static implicit operator Vector2(TilePos tilePos)
		{
			return new Vector2(tilePos.x, tilePos.y);
		}
		
		#endregion
		
		#region World Position Conversion
		
		public static Vector3 ToWorld(TilePos pos, Vector2 tileSize)
		{
			var worldX = (pos.x - pos.y) * tileSize.x * 0.5f;
			var worldY = (pos.x + pos.y) * tileSize.y * 0.5f;
			
			return new Vector3(worldX, worldY, 0);
		}
		
		#endregion
	}
}