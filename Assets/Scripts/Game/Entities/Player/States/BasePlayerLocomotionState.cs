using UnityEngine;

namespace Game.Player.States
{
	public class BasePlayerLocomotionState : BasePlayerState
	{
		public BasePlayerLocomotionState(PlayerController playerController, Animator animator) : base(playerController, animator) { }

		public override void FixedUpdate()
		{
			PlayerController.ApplyMovement();
		}
	}
}