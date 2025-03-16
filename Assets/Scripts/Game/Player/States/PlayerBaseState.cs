using MHL.Core.StateMachine;
using UnityEngine;

namespace MHL.Game.Player.States
{
	public abstract class PlayerBaseState : IState
	{
		protected readonly PlayerController PlayerController;
		protected readonly Animator Animator;

		protected const float CrossFadeDuration = 0.1f;
		
		protected PlayerBaseState(PlayerController playerController, Animator animator)
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