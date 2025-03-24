using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
	public class Chunk
	{
		// Chunk properties
        public Vector2Int Coord;
        public GameObject ChunkObject;
        public Dictionary<Vector2Int, GameObject> TileObjects = new Dictionary<Vector2Int, GameObject>();
        public bool IsActive;
        
        // World reference info
        private readonly int _chunkSize;
        private readonly Transform _parent;
        
        public Chunk(Vector2Int coord, int chunkSize, Transform parent)
        {
            Coord = coord;
            _chunkSize = chunkSize;
            _parent = parent;
            IsActive = false;
        }
        
        public void Activate()
        {
            if (IsActive) return;
            
            if (ChunkObject == null)
            {
                ChunkObject = new GameObject($"Chunk_{Coord.x}_{Coord.y}");
                ChunkObject.transform.parent = _parent;
            }
            
            ChunkObject.SetActive(true);
            IsActive = true;
        }
        
        public void Deactivate()
        {
            if (!IsActive) return;
            
            if (ChunkObject != null)
            {
                ChunkObject.SetActive(false);
            }
            
            IsActive = false;
        }
        
        // Helper method to get world bounds of this chunk
        public Bounds GetWorldBounds(WorldSettingsSO settings)
        {
            // Get grid bounds
            int startX = Coord.x * _chunkSize;
            int startY = Coord.y * _chunkSize;
            int endX = startX + _chunkSize;
            int endY = startY + _chunkSize;
            
            // Convert to isometric coordinates
            Vector2 minWorld = GridToIsometric(new Vector2Int(startX, startY), settings);
            Vector2 maxWorld = GridToIsometric(new Vector2Int(endX, endY), settings);
            
            // Create bounds
            Vector3 center = new Vector3((minWorld.x + maxWorld.x) / 2, (minWorld.y + maxWorld.y) / 2, 0);
            Vector3 size = new Vector3(Mathf.Abs(maxWorld.x - minWorld.x), Mathf.Abs(maxWorld.y - minWorld.y), 1);
            
            return new Bounds(center, size);
        }
        
        // Convert grid coordinates to isometric world coordinates
        private Vector2 GridToIsometric(Vector2Int grid, WorldSettingsSO settings)
        {
            float isometricX = (grid.x - grid.y) * (settings.tileWidth / 2);
            float isometricY = (grid.x + grid.y) * (settings.tileHeight / 2);
            
            return new Vector2(isometricX, isometricY);
        }
	}
}