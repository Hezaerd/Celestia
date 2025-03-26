using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
	[CreateAssetMenu(fileName = "New Tile", menuName = "Game/World/Tile", order = 0)]
	public class TileData : ScriptableObject
	{
		[Title("Tile properties", "", TitleAlignments.Centered)]
		public string tileName;
		
		[PreviewField(ObjectFieldAlignment.Center, Height = 64)]
		public Sprite tileSprite;
	}
}