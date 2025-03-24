using UnityEngine;

namespace Game.World
{
	public struct TileData
	{
		public Vector2Int Position;
		public Vector2 WorldPosition;
		public TerrainTypeSO TerrainType;
		public float NoiseValue;
	}
}