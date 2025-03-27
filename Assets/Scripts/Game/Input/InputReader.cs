using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Input
{
	[CreateAssetMenu(fileName = "New Input Reader", menuName = "Game/Input Reader")]
	public class InputReader : ScriptableObject, GameInput.IGameplayActions
	{
		private GameInput _gameInput;
		
		// Gameplay
		public event UnityAction<Vector2> MoveEvent = delegate { };
		public event Action<float> ZoomEvent = delegate { };
		public event Action PrimaryActionEvent = delegate { };
		public event Action SecondaryActionEvent = delegate { };
		public event Action DodgeEvent = delegate { };
		public event Action InteractEvent = delegate { };
		public event Action StartedRunningEvent = delegate { };
		public event Action StoppedRunningEvent = delegate { };

		private void OnEnable()
		{
			// if (_gameInput != null)
			// 	return;
			
			_gameInput = new GameInput();
			_gameInput.Gameplay.SetCallbacks(this);
			
			Debug.Log("Input Reader Enabled");
		}

		private void OnDisable()
		{
			_gameInput.Disable();
			
			Debug.Log("Input Reader Disabled");
		}
		
		public void EnableGameplayInput()
		{
			_gameInput.Gameplay.Enable();
		}
		
		public void OnMove(InputAction.CallbackContext context)
		{
			MoveEvent.Invoke(context.ReadValue<Vector2>());
		}
		
		public void OnZoom(InputAction.CallbackContext context)
		{
			ZoomEvent.Invoke(context.ReadValue<float>());
		}
		
		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
				InteractEvent.Invoke();
		}
		
		public void OnRun(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					StartedRunningEvent.Invoke();
					break;
				case InputActionPhase.Canceled:
					StoppedRunningEvent.Invoke();
					break;
			}
		}
		
		public void OnDodge(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
				DodgeEvent.Invoke();
		}
		
		public void OnPrimaryAction(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
				PrimaryActionEvent.Invoke();
		}
		
		public void OnSecondaryAction(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
				SecondaryActionEvent.Invoke();
		}
	}
}