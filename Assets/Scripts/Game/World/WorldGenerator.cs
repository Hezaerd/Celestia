using UnityEngine;

namespace Game.World
{
	public class WorldGenerator : MonoBehaviour
	{
		[Header("Tile Prefab & Properties")]
		[SerializeField] private Tile tilePrefab;
		[SerializeField] private TileData tileData;
		
		[Header("Grid Settings")]
		[SerializeField] private int width = 10;
		[SerializeField] private int height = 10;

		private TileGrid _tileGrid;

		private void Awake()
		{
			_tileGrid = new TileGrid(tilePrefab, transform);
		}

		private void Start()
		{
			GenerateWorld();
		}

		private void GenerateWorld()
		{
			// Get offset for centering grid
			var offsetX = width / 2;
			var offsetY = height / 2;
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					TilePos pos = new TilePos(x - offsetX, y - offsetY);
					Tile tile = _tileGrid.CreateTile(pos, tileData);
				}
			}
		}
	}
}