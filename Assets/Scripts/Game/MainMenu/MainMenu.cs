using System;
using Core.StateMachine;
using game.menu.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace game.menu
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField, Required] private GameObject debugPanel;
		[SerializeField, Required] private GameObject homePanel;
		[SerializeField, Required] private GameObject soloPanel;
		[SerializeField, Required] private GameObject multiPanel;
		[SerializeField, Required] private GameObject optionsMenu;
		
		private StateMachine _stateMachine;
		
		// Home Panel flags
		private bool _homeSoloButtonClicked;
		private bool _homeMultiButtonClicked;
		private bool _homeOptionsButtonClicked;
		
		private void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
		
		private void Awake()
		{
			SetupStateMachine();
			SetupDebugPanel();
		}

		private void SetupStateMachine()
		{
			_stateMachine = new StateMachine();

			var mainMenuState = new MainMenuState(homePanel);
			var soloMenuState = new SoloMenuState(soloPanel);
			var multiMenuState = new MultiMenuState(multiPanel);
			var optionsMenuState = new OptionsMenuState(optionsMenu);
			
			_stateMachine.RegisterState(mainMenuState);
			_stateMachine.RegisterState(soloMenuState);
			_stateMachine.RegisterState(multiMenuState);
			_stateMachine.RegisterState(optionsMenuState);
			
			// from MainMenu
			At(mainMenuState, soloMenuState, () => _homeSoloButtonClicked);
			At(mainMenuState, multiMenuState, () => _homeMultiButtonClicked);
			At(mainMenuState, optionsMenuState, () => _homeOptionsButtonClicked);
			
			_stateMachine.OnStateChanged += ResetFlags;
			
			_stateMachine.SetRoot(mainMenuState);
		}

		private void SetupDebugPanel()
		{
			var debugPanelController = debugPanel.GetComponent<DebugPanelController>();
			debugPanelController.Setup(_stateMachine);
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
			// home panel flags
			_homeSoloButtonClicked = false;
			_homeMultiButtonClicked = false;
			_homeOptionsButtonClicked = false;
		}
		
		#region Buttons Callbacks
		
		// Generic Buttons
		public void OnBackButtonClicked() => _stateMachine.GoToPreviousState();
		
		// Home Panel
		public void OnOptionsButtonClicked() => _homeOptionsButtonClicked = true;
		public void OnSoloButtonClicked() => _homeSoloButtonClicked = true;
		public void OnMultiButtonClicked() => _homeMultiButtonClicked = true;
		public void OnExitButtonClicked() => Application.Quit();
		
		#endregion
		
	}
}