using UnityEngine;

namespace Game.World
{
	public class TileComponent : MonoBehaviour
	{
        [Header("References")]
        public SpriteRenderer spriteRenderer;
    
        [Header("Tile Data")]
        public Vector2Int gridPosition;
        public TerrainTypeSO terrainType;
    
        private void Awake()
        {
            // Auto-find renderer if not assigned
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
    
        public void Initialize(TileData data)
        {
            // Store the data locally
            gridPosition = data.Position;
            terrainType = data.TerrainType;
        
            // Update visual representation
            UpdateVisuals();
        }
    
        public void UpdateVisuals()
        {
            if (spriteRenderer == null || terrainType == null)
                return;
            
            // Set sprite
            spriteRenderer.sprite = terrainType.tileSprite;
        }
    
        // Utility methods for game interactions
        public bool IsWalkable()
        {
            return terrainType != null && terrainType.isWalkable;
        }
    
        public float GetMovementCost()
        {
            return terrainType != null ? terrainType.movementCost : 1f;
        }
	}
}