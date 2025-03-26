using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
	/// <summary>
	/// Represents a grid of tiles in the game world.
	/// Handles creation, manipulation and storage of tiles.
	/// </summary>
	public class TileGrid
	{
		private readonly Tile _tilePrefab;
		private readonly Vector2 _tileSize;
		private readonly Transform _tileContainer;
		private readonly Dictionary<TilePos, Tile> _tiles = new Dictionary<TilePos, Tile>();
		
		public TileGrid(Tile tilePrefab, Vector2 tileSize, Transform tileContainer)
		{
			_tilePrefab = tilePrefab;
			_tileSize = tileSize;
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
			newTile.transform.position = TilePos.ToWorld(position, _tileSize);
			newTile.ApplyData(tileData);
			newTile.name = $"{tileData.tileName} ({position})";
			
			_tiles.Add(position, newTile);
			
			return tile;
		}
	}
}