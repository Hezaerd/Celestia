using celestia.game.client;
using Sirenix.OdinInspector;
using UnityEngine;

namespace celestia.game.shared
{
	[CreateAssetMenu(fileName = "New World Settings", menuName = "Game/World/World Settings", order = 0)]
	public class WorldSettingsSO : ScriptableObject
	{
		[BoxGroup("World")]
		[FoldoutGroup("World/Dimensions")]
		public int width = 10;

		[FoldoutGroup("World/Dimensions")]
		public int height = 10;

		[FoldoutGroup("World/Generation")]
		public Tile tilePrefab;

		[FoldoutGroup("World/Generation")]
		public TileData devTileData;
	}
}