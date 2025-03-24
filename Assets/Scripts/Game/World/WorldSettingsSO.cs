using UnityEngine;

namespace Game.World
{
	[CreateAssetMenu(fileName = "New World Settings", menuName = "World Generation/World Settings")]
	public class WorldSettingsSO : ScriptableObject
	{
		[Header("World Dimensions")]
		public int width = 100;
		public int height = 100;
    
		[Header("Seed Settings")]
		public int seed = 12345;
		public bool useRandomSeed = false;
    
		[Header("Tile Settings")]
		public float tileWidth = 1f;
		public float tileHeight = 0.5f;
    
		[Header("Noise Settings")]
		public float noiseScale = 0.1f;
		public int octaves = 4;
		public float persistence = 0.5f;
		public float lacunarity = 2f;
    
		[Header("Terrain Types")]
		public TerrainTypeSO[] terrainTypes;
		public TerrainTypeSO defaultTerrainType;
    
		[Header("Chunk Settings")]
		public int chunkSize = 16;
		public int viewDistance = 3; // In chunks
    
		// Calculate the actual seed to use
		public int GetSeed()
		{
			return useRandomSeed ? Random.Range(0, 999999) : seed;
		}
	}
}