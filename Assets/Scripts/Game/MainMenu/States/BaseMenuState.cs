using Core.StateMachine;
using UnityEngine;

namespace game.menu.States
{
	public abstract class BaseMenuState : IState
	{
		protected readonly GameObject Menu;

		protected BaseMenuState(GameObject menu)
		{
			Menu = menu;
		}
		
		public virtual void OnEnter() { }
		public virtual void Update() { }
		public virtual void FixedUpdate() { }
		public virtual void OnExit() { }
	}
}