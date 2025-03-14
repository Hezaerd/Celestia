using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core
{
	public class DebugBootstrapper : MonoBehaviour
	{
		[BoxGroup("Debug Settings")]
		[SerializeField] private bool createGameProvider = true;

		private void Start()
		{
			if (createGameProvider)
			{
				Debug.Log("[DebugBootstrapper] Creating GameProvider.");
				GameObject go = new GameObject("GameProvider (Debug)");
				go.AddComponent<GameProvider>();
			}
			
			// Destroy this bootstrapper
			Destroy(gameObject);
		}
	}
}