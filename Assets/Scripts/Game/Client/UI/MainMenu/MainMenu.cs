using System;
using celestia.utils.statemachine;
using UnityEngine;

namespace celestia.game.client.ui.mainmenu
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField]private GameObject homePanel;
		[SerializeField]private GameObject hostPanel;
		[SerializeField]private GameObject joinPanel;

		private StateMachine _stateMachine;

		// HomePanel flags
		private bool _homeHostButtonPressed;
		private bool _homeJoinButtonPressed;
		
		private void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
		
		private void Awake()
		{
			_stateMachine = new StateMachine();
			
			var mainMenuState = new MainMenuState(homePanel);
			var hostMenuState = new HostMenuState(hostPanel);
			var joinMenuState = new JoinMenuState(joinPanel);

			_stateMachine.RegisterState(mainMenuState);
			_stateMachine.RegisterState(hostMenuState);
			_stateMachine.RegisterState(joinMenuState);
			
			// Transitions
			At(mainMenuState, hostMenuState, () => _homeHostButtonPressed);
			At(mainMenuState, joinMenuState, () => _homeJoinButtonPressed);

			_stateMachine.OnStateChanged += ResetFlags;
			
			_stateMachine.SetRoot(mainMenuState);
		}

		private void OnDestroy()
		{
			_stateMachine.OnStateChanged -= ResetFlags;
		}

		private void Update()
		{
			_stateMachine.Update();
		}

		private void ResetFlags(IState from, IState to)
		{
			_homeHostButtonPressed = false;
			_homeJoinButtonPressed = false;
		}
		
		#region Button Callbacks
		
		// Generic buttons
		public void OnBackButtonPressed() => _stateMachine.GoToPreviousState();
		
		// Home Panel buttons
		public void SwitchToHostPanel() => _homeHostButtonPressed = true;
		public void SwitchToJoinPanel() => _homeJoinButtonPressed = true;
		
		#endregion
	}
}