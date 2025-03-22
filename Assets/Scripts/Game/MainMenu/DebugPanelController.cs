using Core.StateMachine;
using TMPro;
using UnityEngine;

namespace Game.MainMenu
{
	public class DebugPanelController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI stateHistoryText;

		private StateMachine _menuStateMachine;

		public void Setup(StateMachine stateMachine)
		{
			_menuStateMachine = stateMachine;
			
#if !UNITY_EDITOR
			Destroy(this);
#endif
		}
		
		private void Update()
		{
			if (_menuStateMachine == null)
				return;

			var fullRoute = _menuStateMachine.GetFullStateRoute();
			var stateHistory = "";
			foreach (IState state in fullRoute)
			{
				stateHistory += state.GetType().Name + " > ";
			}
			
			if (stateHistory.EndsWith(" > "))
				stateHistory = stateHistory[..^3];
			
			stateHistoryText.text = stateHistory;			
		}
	}
}