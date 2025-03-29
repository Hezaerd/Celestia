using celestia.utils.statemachine;
using UnityEngine;

namespace celestia.game.client.ui.mainmenu
{
	public abstract class BaseMenuState : IState
	{
		protected readonly GameObject Menu;

		protected BaseMenuState(GameObject menu) => Menu = menu;
		
		public virtual void OnEnter() => Menu.SetActive(true);
		public virtual void OnExit() => Menu.SetActive(false);
		public void Update() { }
		public void FixedUpdate() { }
	}
}