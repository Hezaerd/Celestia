using UnityEngine;

namespace Game.MainMenu.States
{
	public class MainMenuState : BaseMenuState
	{
		public MainMenuState(GameObject menu) : base(menu) { }

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