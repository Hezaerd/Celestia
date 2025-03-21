using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
	public class InputController : MonoBehaviour, PlayerControls_Actions.IPlayerActions
	{
		private PlayerControls_Actions _actions;
		private PlayerControls_Actions.PlayerActions _player;

		// Input properties
		public Vector2 MoveInput { get; private set; }
		public Vector2 LookInput { get; private set; }
		public bool IsRunPressed { get; private set; }
		public bool DodgePressed { get; private set; }
		public bool PrimaryActionPressed { get; private set; }
		public bool SecondaryActionPressed { get; private set; }
		public bool SpecialActionPressed { get; private set; }

		// Interact properties
		public bool IsInteractPressed { get; private set; }
		public float InteractionHoldTime { get; private set; }
		
		// Input buffers for combo detection
		private readonly Queue<BufferedInput> _recentInputs = new Queue<BufferedInput>();
		private const float InputBufferTime = 0.5f;

		private void Awake()
		{
			// Create the controls wrapper using the asset
			_actions = new PlayerControls_Actions();
			_player = _actions.Player;
			_player.AddCallbacks(this);
		}
		
		private void OnDestroy() => _actions.Dispose();
		private void OnEnable() => _player.Enable();
		private void OnDisable() => _player.Disable();

		#region Interface Implementation
		public void OnMove(InputAction.CallbackContext context)
		{
			if(context.performed)
				MoveInput = context.ReadValue<Vector2>();
			else if (context.canceled)
				MoveInput = Vector2.zero;
		}
		
		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				IsInteractPressed = true;
				InteractionHoldTime = 0f;
			}
			else if (context.canceled)
			{
				IsInteractPressed = false;
				InteractionHoldTime = 0f;
			}
		}
		
		public void OnDodge(InputAction.CallbackContext context)
		{
			if (context.performed)
				DodgePressed = true;
		}
		
		public void OnPrimaryAction(InputAction.CallbackContext context)
		{
			if (!context.performed)
				return;
			
			PrimaryActionPressed = true;
			BufferInput(ComboInputAction.PrimaryAction);
		}
		
		public void OnSecondaryAction(InputAction.CallbackContext context)
		{
			if (!context.performed)
				return;
			
			SecondaryActionPressed = true;
			BufferInput(ComboInputAction.SecondaryAction);
		}
		
		public void OnSpecialAction(InputAction.CallbackContext context)
		{
			if (!context.performed)
				return;
			
			SpecialActionPressed = true;
			BufferInput(ComboInputAction.SpecialAction);
		}
		
		public void OnRun(InputAction.CallbackContext context)
		{
			if (context.performed)
				IsRunPressed = true;
			else if (context.canceled)
				IsRunPressed = false;
		}
		
		public void OnZoom(InputAction.CallbackContext context)
		{
			// Not implemented
		}
		#endregion
		
		private void Update()
		{
			// Reset one-frame actions.
			DodgePressed = false;
			PrimaryActionPressed = false;
			SecondaryActionPressed = false;
			SpecialActionPressed = false;

			// Update interaction hold time
			if (IsInteractPressed)
				InteractionHoldTime += Time.deltaTime;
			
			// Clean up old inputs from buffer
			CleanInputBuffer();
		}

		private void BufferInput(ComboInputAction action)
		{
			_recentInputs.Enqueue(new BufferedInput { Action = action, Timestamp = Time.time });
		}

		private void CleanInputBuffer()
		{
			while (_recentInputs.Count > 0 && Time.time - _recentInputs.Peek().Timestamp > InputBufferTime)
			{
				_recentInputs.Dequeue();
			}
		}

		private struct BufferedInput
		{
			public ComboInputAction Action;
			public float Timestamp;
		}

		public bool CheckCombo(ComboInputAction[] sequence)
		{
			if (_recentInputs.Count < sequence.Length)
				return false;

			var inputs = _recentInputs.ToArray();
			for (var i = 0; i < sequence.Length; i++)
			{
				if (inputs[i].Action != sequence[i])
					return false;
			}

			return true;
		}

		public enum ComboInputAction
		{
			PrimaryAction,
			SecondaryAction,
			SpecialAction
		}
	}
}