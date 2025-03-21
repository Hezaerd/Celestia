using UnityEngine;

namespace Game.Player.States
{
	public class PlayerLocomotionState : PlayerBaseState
	{
		public PlayerLocomotionState(PlayerController playerController, Animator animator) : base(playerController, animator) { }

		public override void FixedUpdate()
		{
			PlayerController.ApplyMovement();
		}
	}
}