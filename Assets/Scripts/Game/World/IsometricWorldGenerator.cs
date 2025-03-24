using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
	public class IsometricWorldGenerator : MonoBehaviour
	{
		[Header("World Generation")]
        public WorldSettingsSO worldSettings;
        
        [Header("Runtime References")]
        public Transform player;
        
        [Header("Development")]
        public bool generateOnStart = true;
        public bool showChunkGizmos;
        
        // World data
        private TileData[,] _tileData;
        private readonly Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();
        private Vector2Int _currentChunkCoord;
        private int _currentSeed;
        
        // Generation state
        private bool _isGenerating;
        private bool _worldDataGenerated;

        private void Start()
        {
            if (generateOnStart)
                GenerateWorld();
        }

        private void Update()
        {
            if (!_worldDataGenerated || !player)
                return;
                
            // Check if player moved to a new chunk
            Vector2Int playerChunkCoord = WorldToChunkCoord(player.position);
            
            if (playerChunkCoord != _currentChunkCoord)
            {
                _currentChunkCoord = playerChunkCoord;
                UpdateChunks();
            }
        }
        
        public void GenerateWorld()
        {
            if (_isGenerating)
                return;
                
            StartCoroutine(GenerateWorldCoroutine());
        }
        
        private IEnumerator GenerateWorldCoroutine()
        {
            _isGenerating = true;
            _currentSeed = worldSettings.GetSeed();
            
            Debug.Log($"Generating world with seed: {_currentSeed}");
            
            // Initialize tile data array
            _tileData = new TileData[worldSettings.width, worldSettings.height];
            
            // Generate the noise map
            yield return StartCoroutine(GenerateNoiseMap());
            
            // Mark world data as generated
            _worldDataGenerated = true;
            
            // Initial chunk loading
            if (player != null)
            {
                _currentChunkCoord = WorldToChunkCoord(player.position);
                UpdateChunks();
            }
            
            _isGenerating = false;
            Debug.Log("World generation complete");
        }
        
        private IEnumerator GenerateNoiseMap()
        {
            System.Random prng = new System.Random(_currentSeed);
            
            // Create random offsets for each octave to add variation
            Vector2[] octaveOffsets = new Vector2[worldSettings.octaves];
            for (int i = 0; i < worldSettings.octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000);
                float offsetY = prng.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            
            // Process the tiles in batches to prevent frame freezing
            int tilesPerFrame = 1000;
            int tilesProcessed = 0;
            
            for (int y = 0; y < worldSettings.height; y++)
            {
                for (int x = 0; x < worldSettings.width; x++)
                {
                    // Generate the noise value
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;
                    float maxValue = 0f;
                    
                    for (int i = 0; i < worldSettings.octaves; i++)
                    {
                        float sampleX = (x + octaveOffsets[i].x) * worldSettings.noiseScale * frequency;
                        float sampleY = (y + octaveOffsets[i].y) * worldSettings.noiseScale * frequency;
                        
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        
                        maxValue += amplitude;
                        amplitude *= worldSettings.persistence;
                        frequency *= worldSettings.lacunarity;
                    }
                    
                    // Normalize the noise value
                    float normalizedHeight = Mathf.Clamp01((noiseHeight + 1) / (maxValue / 0.9f));
                    
                    // Calculate the isometric position
                    Vector2 worldPosition = GridToIsometric(new Vector2Int(x, y));
                    
                    // Create tile data
                    _tileData[x, y] = new TileData
                    {
                        Position = new Vector2Int(x, y),
                        WorldPosition = worldPosition,
                        NoiseValue = normalizedHeight,
                        TerrainType = GetTerrainTypeForNoise(normalizedHeight)
                    };
                    
                    // Increment processed tiles counter
                    tilesProcessed++;
                    if (tilesProcessed >= tilesPerFrame)
                    {
                        tilesProcessed = 0;
                        yield return null;
                    }
                }
            }
        }
        
        private TerrainTypeSO GetTerrainTypeForNoise(float noiseValue)
        {
            if (worldSettings.terrainTypes == null || worldSettings.terrainTypes.Length == 0)
                return worldSettings.defaultTerrainType;
                
            foreach (TerrainTypeSO terrainType in worldSettings.terrainTypes)
            {
                if (noiseValue >= terrainType.minThreshold && noiseValue <= terrainType.maxThreshold)
                    return terrainType;
            }
            
            return worldSettings.defaultTerrainType;
        }
        
        private void UpdateChunks()
        {
            // Calculate which chunks should be visible
            HashSet<Vector2Int> chunksToShow = new HashSet<Vector2Int>();
            
            for (int y = -worldSettings.viewDistance; y <= worldSettings.viewDistance; y++)
            {
                for (int x = -worldSettings.viewDistance; x <= worldSettings.viewDistance; x++)
                {
                    Vector2Int chunkCoord = new Vector2Int(_currentChunkCoord.x + x, _currentChunkCoord.y + y);
                    
                    // Skip invalid chunks (outside world boundaries)
                    if (IsChunkOutOfBounds(chunkCoord))
                        continue;
                    
                    chunksToShow.Add(chunkCoord);
                    
                    // Create chunk if it doesn't exist yet
                    if (!_chunks.ContainsKey(chunkCoord))
                    {
                        Chunk newChunk = new Chunk(chunkCoord, worldSettings.chunkSize, transform);
                        _chunks.Add(chunkCoord, newChunk);
                    }
                }
            }
            
            // Activate or deactivate chunks
            foreach (var chunk in _chunks.Values)
            {
                if (chunksToShow.Contains(chunk.Coord))
                {
                    if (!chunk.IsActive)
                    {
                        chunk.Activate();
                        StartCoroutine(PopulateChunk(chunk));
                    }
                }
                else
                {
                    if (chunk.IsActive)
                    {
                        chunk.Deactivate();
                    }
                }
            }
        }
        
        private IEnumerator PopulateChunk(Chunk chunk)
        {
            // Calculate the grid range for this chunk
            int startX = chunk.Coord.x * worldSettings.chunkSize;
            int startY = chunk.Coord.y * worldSettings.chunkSize;
            int endX = Mathf.Min(startX + worldSettings.chunkSize, worldSettings.width);
            int endY = Mathf.Min(startY + worldSettings.chunkSize, worldSettings.height);
            
            // Skip if outside world bounds
            if (startX >= worldSettings.width || startY >= worldSettings.height || endX <= 0 || endY <= 0)
                yield break;
            
            // Clamp to world boundaries
            startX = Mathf.Max(0, startX);
            startY = Mathf.Max(0, startY);
            
            // Process tiles in batches
            int tilesPerFrame = 20;
            int tilesProcessed = 0;
            
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    // Get the tile data
                    TileData data = _tileData[x, y];
                    Vector2Int gridPos = new Vector2Int(x, y);
                    
                    // Skip if tile already exists in this chunk
                    if (chunk.TileObjects.ContainsKey(gridPos))
                        continue;
                        
                    // Skip if no terrain type assigned
                    if (!data.TerrainType)
                        continue;
                        
                    // Instantiate the tile prefab
                    if (data.TerrainType.tilePrefab)
                    {
                        Vector3 position = new Vector3(data.WorldPosition.x, data.WorldPosition.y, 0);
                        GameObject tilePrefab = data.TerrainType.tilePrefab;
                        
                        GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, chunk.ChunkObject.transform);
                        tileObject.name = $"Tile_{x}_{y}";
                        
                        // Configure sprite renderer
                        SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
                        if (spriteRenderer)
                        {
                            // Sort tiles for proper isometric stacking
                            spriteRenderer.sortingOrder = worldSettings.width + worldSettings.height - x - y;
                        }
                        
                        // Initialize tile component
                        TileComponent tileComp = tileObject.GetComponent<TileComponent>();
                        if (tileComp)
                        {
                            tileComp.Initialize(data);
                        }
                        
                        // Store reference to this tile
                        chunk.TileObjects[gridPos] = tileObject;
                        
                        tilesProcessed++;
                        if (tilesProcessed >= tilesPerFrame)
                        {
                            tilesProcessed = 0;
                            yield return null;
                        }
                    }
                }
            }
        }
        
        // Convert grid coordinates to isometric world coordinates
        public Vector2 GridToIsometric(Vector2Int grid)
        {
            float isometricX = (grid.x - grid.y) * (worldSettings.tileWidth / 2);
            float isometricY = (grid.x + grid.y) * (worldSettings.tileHeight / 2);
            
            return new Vector2(isometricX, isometricY);
        }
        
        // Convert world coordinates to grid coordinates
        public Vector2Int IsometricToGrid(Vector2 world)
        {
            float gridX = (world.x / (worldSettings.tileWidth / 2) + world.y / (worldSettings.tileHeight / 2)) / 2;
            float gridY = (world.y / (worldSettings.tileHeight / 2) - world.x / (worldSettings.tileWidth / 2)) / 2;
            
            return new Vector2Int(Mathf.RoundToInt(gridX), Mathf.RoundToInt(gridY));
        }
        
        // Convert world position to chunk coordinate
        public Vector2Int WorldToChunkCoord(Vector3 worldPosition)
        {
            // Convert to grid position first
            Vector2Int gridPos = IsometricToGrid(new Vector2(worldPosition.x, worldPosition.y));
            
            // Then to chunk position
            return new Vector2Int(
                Mathf.FloorToInt(gridPos.x / (float)worldSettings.chunkSize),
                Mathf.FloorToInt(gridPos.y / (float)worldSettings.chunkSize)
            );
        }
        
        // Check if a chunk would be outside world boundaries
        private bool IsChunkOutOfBounds(Vector2Int chunkCoord)
        {
            int startX = chunkCoord.x * worldSettings.chunkSize;
            int startY = chunkCoord.y * worldSettings.chunkSize;
            
            return startX >= worldSettings.width || 
                   startY >= worldSettings.height || 
                   startX + worldSettings.chunkSize <= 0 || 
                   startY + worldSettings.chunkSize <= 0;
        }
        
        // Clear the entire world
        public void ClearWorld()
        {
            StopAllCoroutines();
            
            foreach (var chunk in _chunks.Values)
            {
                if (chunk.ChunkObject != null)
                    Destroy(chunk.ChunkObject);
            }
            
            _chunks.Clear();
            _tileData = null;
            _worldDataGenerated = false;
        }
        
        private void OnDrawGizmos()
        {
            if (!showChunkGizmos || _chunks == null)
                return;
                
            foreach (var chunk in _chunks.Values)
            {
                // Set color based on active state
                Gizmos.color = chunk.IsActive ? Color.green : Color.red;
                
                // Draw chunk bounds
                Bounds bounds = chunk.GetWorldBounds(worldSettings);
                Gizmos.DrawWireCube(bounds.center, bounds.size);
                
                // Draw chunk label
                if (chunk.IsActive)
                {
                    UnityEditor.Handles.Label(bounds.center, $"Chunk {chunk.Coord.x}, {chunk.Coord.y}");
                }
            }
            
            // Highlight current chunk
            if (_worldDataGenerated)
            {
                if (_chunks.TryGetValue(_currentChunkCoord, out Chunk currentChunk))
                {
                    Gizmos.color = Color.yellow;
                    Bounds bounds = currentChunk.GetWorldBounds(worldSettings);
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }
            }
        }
	}
}