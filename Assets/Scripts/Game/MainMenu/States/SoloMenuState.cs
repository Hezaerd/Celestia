using UnityEngine;

namespace Game.MainMenu.States
{
	public class SoloMenuState : BaseMenuState
	{
		public SoloMenuState(GameObject menu) : base(menu) { }
		
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