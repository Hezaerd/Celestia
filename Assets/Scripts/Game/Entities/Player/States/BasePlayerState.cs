using Core.StateMachine;
using UnityEngine;

namespace Game.Player.States
{
	public abstract class BasePlayerState : IState
	{
		protected readonly PlayerController PlayerController;
		protected readonly Animator Animator;
		
		protected BasePlayerState(PlayerController playerController, Animator animator)
		{
			PlayerController = playerController;
			Animator = animator;
		}

		public virtual void OnEnter() { }
		public virtual void Update() { }
		public virtual void FixedUpdate() { }
		public virtual void OnExit() { }
	}
}