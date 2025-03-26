using KBCore.Refs;
using UnityEngine;

namespace Game.World
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class Tile : MonoBehaviour
	{
		private string _tileName;

		[SerializeField, Self, HideInInspector]
		private SpriteRenderer spriteRenderer;
		
		public void ApplyData(TileData tileData)
		{
			_tileName = tileData.tileName;
			spriteRenderer.sprite = tileData.tileSprite;
		}
		
		private void OnValidate()
		{
			this.ValidateRefs();
		}
	}
}