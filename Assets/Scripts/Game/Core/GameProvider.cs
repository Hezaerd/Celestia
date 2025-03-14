using MHL.Core.DependencyInjection;
using UnityEngine;

namespace Game.Core
{
	[DefaultExecutionOrder(-50)] // Ensure this runs AFTER AppProvider
	public class GameProvider : MonoProvider
	{
		public override void RegisterDependencies(IDependencyContainer container)
		{
			// Register game dependencies here

			Debug.Log("[GameProvider] Successfully registered dependencies.");
		}
	}
}