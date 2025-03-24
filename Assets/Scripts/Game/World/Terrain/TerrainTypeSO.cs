using UnityEngine;

namespace Game.World
{
	[CreateAssetMenu(fileName = "New Terrain Type", menuName = "World Generation/Terrain Type")]
	public class TerrainTypeSO : ScriptableObject
	{
		public string terrainName;
		public Sprite tileSprite;
		public GameObject tilePrefab;
		public Color mapColor = Color.white;
    
		[Header("Terrain Properties")]
		public bool isWalkable = true;
		public float movementCost = 1f;
    
		[Header("Noise Threshold")]
		[Range(0f, 1f)] public float minThreshold = 0f;
		[Range(0f, 1f)] public float maxThreshold = 1f;
	}
}