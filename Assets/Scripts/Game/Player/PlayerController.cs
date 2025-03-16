using System;
using KBCore.Refs;
using MHL.Game.Entities.Components;
using MHL.Core.StateMachine;
using MHL.Core.DependencyInjection;
using MHL.Game.Player.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MHL.Game.Player
{
	public class PlayerController : MonoBehaviour
	{
		// Automaticaly Injected dependencies before Awake
		[Inject]
		private readonly InputController _inputController;

		// Properties
		[Space(10)]
		[Title("Player Controller", TitleAlignment = TitleAlignments.Centered)]
		
		[SerializeField] private float maxWalkSpeed = 5f;
		[SerializeField] private float maxRunSpeed = 8f;
		[SerializeField] private float acceleration = 25f;

		// Components
		[SerializeField, Self] private Rigidbody2D rb;
		[SerializeField, Self] private Animator animator;
		[SerializeField, Self] private GroundChecker groundChecker;

		// Movement
		private Vector2 _moveDirection;
		private bool _isRunning;

		// State Machine
		private StateMachine _stateMachine;

		protected void Awake()
		{
			SetupStateMachine();
		}

		private void AddTransition(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
		private void AddAnyTransition(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

		private void SetupStateMachine()
		{
			// Setup state machine
			_stateMachine = new StateMachine();

			var locomotionState = new PlayerLocomotionState(this, animator);
			var dodgeState = new PlayerDodgeState(this, animator);

			AddAnyTransition(locomotionState, new FuncPredicate(CanReturnToLocomotion));
			
			// Set initial state
			_stateMachine.SetState(locomotionState);
		}
		
		private bool CanReturnToLocomotion() => groundChecker.IsGrounded;

		private void Update()
		{
			_stateMachine.Update();
		}

		private void FixedUpdate()
		{
			_stateMachine.FixedUpdate();
		}

		public void ApplyMovement()
		{
			Vector2 moveInput = _inputController.MoveInput;
			var currentHorizontalSpeed = rb.linearVelocityX;

			// Only change horizontal speed if input is provided
			if (Mathf.Abs(moveInput.x) > 0.1f)
			{
				var targetSpeed = _isRunning ? maxRunSpeed : maxWalkSpeed;
				targetSpeed *= Mathf.Abs(moveInput.x);

				var moveDirection = Mathf.Sign(moveInput.x);

				// Accelerate towards the target speed
				currentHorizontalSpeed = Mathf.MoveTowards(
					currentHorizontalSpeed,
					moveDirection * targetSpeed,
					acceleration * Time.fixedDeltaTime
				);

				// Flip the sprite if moving in the opposite direction
				Vector3 scale = transform.localScale;
				scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
				transform.localScale = scale;
			}

			// Apply the calculated velocity
			rb.linearVelocity = new Vector2(currentHorizontalSpeed, rb.linearVelocityY);
		}
		
		private void OnValidate()
		{
			this.ValidateRefs();
		}
	}
}