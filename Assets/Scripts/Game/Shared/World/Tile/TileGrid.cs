using System.Collections.Generic;
using celestia.game.client;
using celestia.math;
using UnityEngine;

namespace celestia.game.shared
{
	/// <summary>
	/// Represents a grid of tiles in the game world.
	/// Handles creation, manipulation and storage of tiles.
	/// </summary>
	public class TileGrid
	{
		private readonly Tile _tilePrefab;
		private readonly Transform _tileContainer;
		private readonly Dictionary<TilePos, Tile> _tiles = new Dictionary<TilePos, Tile>();

		public TileGrid(Tile tilePrefab, Transform tileContainer)
		{
			_tilePrefab = tilePrefab;
			_tileContainer = tileContainer;
		}

		/// <summary>
		/// Get the tile at the specified position.
		/// </summary>
		/// <param name="position">The position of the tile.</param>
		/// <returns>The tile at the position</returns>
		public Tile GetTile(TilePos position)
		{
			_tiles.TryGetValue(position, out Tile tile);
			return tile;
		}

		/// <summary>
		/// Create a new tile at the given grid position.
		/// If a tile already exists at the position, it will be returned.
		/// </summary>
		public Tile CreateTile(TilePos position, TileData tileData)
		{
			if (_tiles.TryGetValue(position, out Tile tile))
				return tile;

			Tile newTile = Object.Instantiate(_tilePrefab, _tileContainer);
			newTile.transform.position = Isometric.TileToWorld(position);
			newTile.ApplyData(tileData);
			newTile.name = $"{tileData.tileName} ({position})";

			_tiles.Add(position, newTile);

			return newTile;
		}
	}
}