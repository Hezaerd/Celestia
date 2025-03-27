using System;
using Game.Player;
using Core.DependencyInjection;
using UnityEngine;

namespace Game.Core
{
	public class GameProvider : MonoProvider
	{
		private static GameProvider _instance;

		protected override void Awake()
		{
			// Check to make sure the class is a valid class instance
			if (_instance != null && _instance != this)
			{
				Destroy(gameObject);
				return;
			}
            
			_instance = this;
			DontDestroyOnLoad(gameObject);
			
			// Setup container and inject dependencies
			base.Awake();
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		public override void RegisterDependencies(IDependencyContainer container)
		{
			// Register core systems
			
			Debug.Log("[GameProvider] Successfully registered dependencies.");
		}
		
		// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (_instance != null)
			{
				Debug.Log($"[GameProvider] Already initialized.");
				return;
			}

			GameObject go = new GameObject("GameProvider");
			_instance = go.AddComponent<GameProvider>();
            
			DontDestroyOnLoad(go);
            
			Debug.Log("[GameProvider] Successfully initialized.");
		}
	}
}