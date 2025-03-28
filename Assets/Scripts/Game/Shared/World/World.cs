using System.Collections.Generic;
using celestia.math;
using UnityEngine;

namespace celestia.game.shared
{
	public class World : MonoBehaviour
	{
		[SerializeField]
		private WorldSettingsSO worldSettings;

		private TileGrid _tileGrid;

		private List<PlayerEntity> _players = new List<PlayerEntity>();

		private void Awake()
		{
			_tileGrid = new TileGrid(worldSettings.tilePrefab, transform);
		}

		private void Start()
		{
			GenerateWorld();
		}

		private void GenerateWorld()
		{
			// Get offset for centering grid
			var offsetX = worldSettings.width / 2;
			var offsetY = worldSettings.height / 2;

			for (var y = 0; y < worldSettings.height; y++)
			{
				for (var x = 0; x < worldSettings.width; x++)
				{
					TilePos pos = new TilePos(x - offsetX, y - offsetY);
					Tile tile = _tileGrid.CreateTile(pos, worldSettings.devTileData);
				}
			}
		}


	}
}