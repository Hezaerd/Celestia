using Game.Player.States;
using KBCore.Refs;
using Core.StateMachine;
using Core.DependencyInjection;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
	[RequireComponent(typeof(Rigidbody2D)) , RequireComponent(typeof(Animator)), RequireComponent(typeof(NetworkObject))]
	public class PlayerController : NetworkBehaviour
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
		// Movement
		private Vector2 _moveDirection;
		private bool _isRunning;

		// State Machine
		private StateMachine _stateMachine;

		protected void Awake()
		{
			SetupStateMachine();
		}
		
		private void SetupStateMachine()
		{
			// Setup state machine
			_stateMachine = new StateMachine();

			var locomotionState = new BasePlayerLocomotionState(this, animator);
			var dodgeState = new BasePlayerDodgeState(this, animator);

			_stateMachine.RegisterState(locomotionState);
			
			_stateMachine.AddAnyTransition(locomotionState, new FuncPredicate(() => _inputController.MoveInput.magnitude > 0));
			
			// Set initial state
			_stateMachine.SetRoot(locomotionState);
		}
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
			moveInput.Normalize();

			rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveInput * (maxWalkSpeed * (_isRunning ? maxRunSpeed : 1)), acceleration * Time.fixedDeltaTime);
		}
		
		private void OnValidate()
		{
			this.ValidateRefs();
		}
	}
}