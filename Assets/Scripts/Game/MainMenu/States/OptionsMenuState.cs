using UnityEngine;

namespace game.menu.States
{
	public class OptionsMenuState : BaseMenuState
	{
		public OptionsMenuState(GameObject menu) : base(menu){ }
		
		public override void OnEnter()
		{
			Menu.SetActive(true);
		}

		public override void OnExit()
		{
			Menu.SetActive(false);
		}
	}
}